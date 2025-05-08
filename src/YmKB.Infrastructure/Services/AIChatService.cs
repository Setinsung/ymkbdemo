using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Text;
using YmKB.Domain.Entities;

namespace YmKB.Infrastructure.Services;

public class AIChatService
{
    
    /// <summary>
    /// 用于将文本切分成段落，每个段落不超过 4000 个字符。
    /// 每个段落作为输入传递给 QA 函数，该函数使用 AI 模型生成答案，最后答案组异步返回
    /// </summary>
    /// <param name="prompt"></param>
    /// <param name="value"></param>
    /// <param name="chatModel"></param>
    /// <param name="aiKernelCreateService"></param>
    /// <returns></returns>
    public static async IAsyncEnumerable<string> QaAsync(string prompt, string value, AIModel chatModel,
        AIKernelCreateService aiKernelCreateService)
    {
        var kernel = aiKernelCreateService.CreateFunctionKernel(chatModel);
        var qaFunction = kernel.CreateFunctionFromPrompt(prompt, functionName: "QA", description: "QA问答");


#pragma warning disable SKEXP0050
        var lines = TextChunker.SplitPlainTextLines(value, 299);
        var paragraphs = TextChunker.SplitPlainTextParagraphs(lines, 4000);
#pragma warning restore SKEXP0050

        foreach (var paragraph in paragraphs)
        {
            var result = await kernel.InvokeAsync(qaFunction, new KernelArguments
            {
                {
                    "input", paragraph
                }
            });

            yield return result.GetValue<string>();
        }
    }
}