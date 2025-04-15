using YmKB.Domain.Abstractions.Common;

namespace YmKB.Domain.Entities;

/// <summary>
/// chat应用
/// </summary>
public class KbApp : BaseAuditableEntity, IAuditTrial, ISoftDelete
{
    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// 图标
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// 类型
    /// </summary>
    public KbAppType KbAppType { get; set; }

    /// <summary>
    /// 会话模型ID
    /// </summary>
    public string? ChatModelId { get; set; }

    /// <summary>
    /// Embedding 模型Id
    /// </summary>
    public string? EmbeddingModelId { get; set; }

    /// <summary>
    /// 温度
    /// </summary>
    public double Temperature { get; set; } = 70f;

    /// <summary>
    /// 提示词
    /// </summary>
    public string? Prompt { get; set; }

    /// <summary>
    /// 插件列表
    /// </summary>
    public string? ApiFunctionList { get; set; }

    /// <summary>
    /// 本地函数列表
    /// </summary>
    public string? NativeFunctionList { get; set; }

    /// <summary>
    /// 知识库ID列表
    /// </summary>
    public string? KbIdList { get; set; }

    /// <summary>
    /// 相似度
    /// </summary>
    public double Relevance { get; set; } = 60f;

    /// <summary>
    /// 提问最大token数
    /// </summary>
    public int MaxAskPromptSize { get; set; } = 2048;
    
    /// <summary>
    /// 向量匹配数
    /// </summary>
    public int MaxMatchesCount { get; set; } = 3;

    /// <summary>
    /// 回答最大token数
    /// </summary>
    public int AnswerTokens { get; set; } = 2048;

    /// <summary>
    /// prompt模板
    /// </summary>
    public string PromptTemplate { get; set; } = string.Empty;
    
    /// <summary>
    /// 未找到的回答模板，如果模板为空则使用Chat对话模型回答。
    /// </summary>
    public string? NoReplyFoundTemplate { get; set; }
    
    public DateTime? Deleted { get; set; }
    public string? DeletedBy { get; set; }
}

public enum KbAppType
{
    /// <summary>
    /// 基本Chat
    /// </summary>
    Chat,
    /// <summary>
    /// 知识库Chat
    /// </summary>
    KbChat
}