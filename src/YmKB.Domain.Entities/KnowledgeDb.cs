using YmKB.Domain.Abstractions.Common;

namespace YmKB.Domain.Entities;

/// <summary>
/// 知识库
/// </summary>
public class KnowledgeDb : BaseAuditableEntity, IAuditTrial, ISoftDelete
{
    /// <summary>
    /// 图标
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// 会话模型ID
    /// </summary>
    public string? ChatModelID { get; set; }

    /// <summary>
    /// 向量模型ID
    /// </summary>
    public string? EmbeddingModelID { get; set; }

    /// <summary>
    /// 每段落的最大标记数
    /// </summary>
    public int MaxTokensPerParagraph { get; set; } = 299;

    /// <summary>
    /// 每句话的最大标记数
    /// </summary>
    public int MaxTokensPerLine { get; set; } = 99;

    /// <summary>
    /// 段落之间重叠标记数量
    /// </summary>
    public int OverlappingTokens { get; set; } = 49;

    public DateTime? Deleted { get; set; }
    public string? DeletedBy { get; set; }
}
