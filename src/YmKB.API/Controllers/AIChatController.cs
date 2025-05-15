using System.Collections.Generic;
using System.Linq;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.KernelMemory;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Data;
using ReallzeV.SemanticKernel.Plugins.Web.ShaBing;
using YmKB.API.Models;
using YmKB.Application.Contracts;
using YmKB.Application.Contracts.Persistence;
using YmKB.Domain.Entities;
using YmKB.JSFunctionCall;

namespace YmKB.API.Controllers;

[ApiController]
[Route("[controller]")]
public class AIChatController : ControllerBase
{
    private readonly ILogger<AIChatController> _logger;
    private readonly IAIKernelCreateService _aiKernelCreateService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IMediator _mediator;
    private static readonly Dictionary<string, ChatHistory> _chatHistories = new();
    private static readonly JsFunctionCallContext _jsFunctionCallContext = new();

    public AIChatController(
        ILogger<AIChatController> logger,
        IAIKernelCreateService aiKernelCreateService,
        IApplicationDbContext dbContext,
        IMediator mediator
    )
    {
        _logger = logger;
        _aiKernelCreateService = aiKernelCreateService;
        _dbContext = dbContext;
        _mediator = mediator;
    }

    [HttpPost("chat")]
    public async Task<ActionResult<ChatResponse>> Chat([FromBody] ChatRequest request)
    {
        try
        {
            // 获取应用信息
            var application = await _dbContext
                .KbApps
                .FirstOrDefaultAsync(a => a.Id == request.ApplicationId);
            if (application == null)
            {
                return NotFound("应用不存在");
            }

            // 获取模型信息
            var chatModel = await _dbContext
                .AIModels
                .FirstOrDefaultAsync(m => m.Id == application.ChatModelId);
            var embeddingModel = await _dbContext
                .AIModels
                .FirstOrDefaultAsync(m => m.Id == application.EmbeddingModelId);

            if (chatModel == null || embeddingModel == null)
            {
                return BadRequest("模型配置错误");
            }

            // 创建Semantic Kernel
            var kernel = _aiKernelCreateService.CreateFunctionKernel(chatModel);
            OpenAIPromptExecutionSettings aiPromptExecutionSettings =
                new()
                {
                    Temperature = application.Temperature,
                    MaxTokens = application.AnswerTokens
                };
            aiPromptExecutionSettings.FunctionChoiceBehavior = FunctionChoiceBehavior.Auto();

            if (request.IsWebTextSearch)
            {
                var textSearch = new ShaBingTextSearch(new ShaBingTextSearchOptions {Logger = _logger});
                var searchPlugin = textSearch.CreateWithSearch("SearchPlugin");
                kernel.Plugins.Add(searchPlugin);
            }

            // 导入kernel插件
            if (!string.IsNullOrWhiteSpace(application.ApiFunctionList))
            {
                var pluginIds = application.ApiFunctionList.Split(", ").Where(s => !string.IsNullOrEmpty(s)).ToHashSet();
                var plugins = await _dbContext
                   .JsFunctionCalls
                   .Where(p => pluginIds.Contains(p.Id))
                   .ToListAsync();
                foreach (var plugin in plugins)
                {
                    var function = kernel.CreateFunctionFromMethod(async (params object[] args) =>
                    {
                        var result = await _jsFunctionCallContext.FunctionCallAsync(plugin.ScriptContent,
                            plugin.MainFuncName,
                            args);
                        return result;
                    },
                    plugin.MainFuncName,
                    plugin.Description,
                    plugin.Parameters.Select(e => new KernelParameterMetadata(e.ParamName)
                    {
                        Name = e.ParamName,
                        Description = e.ParamDescription
                    }));
                    kernel.Plugins.AddFromFunctions(plugin.MainFuncName, plugin.Description,
                        [function]);
                }
            }

            // 获取或创建聊天历史
            var conversationId = request.ConversationId ?? Guid.NewGuid().ToString();
            if (!_chatHistories.TryGetValue(conversationId, out var chatHistory))
            {
                chatHistory = new ChatHistory();
                _chatHistories[conversationId] = chatHistory;
            }

            // 知识库对话
            if (application.KbAppType is KbAppType.KbChat)
            {
                // 创建Kernel Memory服务
                var memory = _aiKernelCreateService.CreateMemoryServerless(
                    chatModel,
                    embeddingModel
                );

                // 解析知识库ID列表
                var kbIds = application.KbIdList.Split(", ").Where(s => !string.IsNullOrEmpty(s)).ToHashSet();
                var allSearchResults = new List<Citation>();

                // 对每个知识库进行搜索
                foreach (var kbId in kbIds)
                {
                    var searchResults = await memory.SearchAsync(
                        request.Message,
                        "kb",
                        new MemoryFilter().ByTag("kbId", kbId),
                        minRelevance: application.Relevance,
                        limit: application.MaxMatchesCount
                    );
                    allSearchResults.AddRange(searchResults.Results);
                }

                // 构建提示词
                var context = string.Join(
                    "\n\n",
                    allSearchResults.SelectMany(r => r.Partitions.Select(p => p.Text))
                );
                var prompt = string.IsNullOrEmpty(application.Prompt)
                    ? $@"你是一个知识库助手。请基于以下参考信息回答问题。如果参考信息不足以回答问题，请说明无法回答。
                        无法回答时，如果允许联网查询，请说明无法回答但通过网络查询得到结果...
                
                参考信息：
                {context}
                
                用户问题：{request.Message}
                
                请回答："
                    : application
                        .Prompt
                        .Replace("{context}", context)
                        .Replace("{question}", request.Message);

                // 获取AI响应
                var chatCompletion = kernel.GetRequiredService<IChatCompletionService>();
                chatHistory.AddUserMessage(prompt);
                var response = await chatCompletion.GetChatMessageContentAsync(
                    chatHistory,
                    aiPromptExecutionSettings,
                    kernel
                );
                // 构建响应
                var chatResponse = new ChatResponse
                {
                    Message = response.Content,
                    ConversationId = conversationId,
                    References = allSearchResults
                        .SelectMany(
                            r =>
                                r.Partitions.Select(
                                    p =>
                                        new ChatReference
                                        {
                                            Content = p.Text,
                                            Source = r.DocumentId
                                        }
                                )
                        )
                        .ToList()
                };

                return Ok(chatResponse);
            }
            else
            {
                // 一般聊天逻辑
                var chatCompletion = kernel.GetRequiredService<IChatCompletionService>();
                // 添加用户消息到对话历史
                chatHistory.AddUserMessage(request.Message);
                // 直接调用AI模型获取回复，不使用知识库参考
                var response = await chatCompletion.GetChatMessageContentAsync(
                    chatHistory,
                    aiPromptExecutionSettings,
                    kernel
                );
                // 添加AI回复到对话历史
                chatHistory.AddAssistantMessage(response.Content);
                // 构建普通聊天响应（不包含知识库引用）
                var chatResponse = new ChatResponse
                {
                    Message = response.Content,
                    ConversationId = conversationId,
                    References =  [ ] // 普通聊天没有知识库引用
                };
                return Ok(chatResponse);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "聊天处理失败");
            return StatusCode(500, "聊天处理失败");
        }
    }
}
