using YmKB.Domain.Abstractions.Common;

namespace YmKB.Domain.Entities;

/// <summary>
/// 文档
/// </summary>
public class KbDocFile : BaseAuditableEntity, IAuditTrial, ISoftDelete
{
    /// <summary>
    /// 知识库id
    /// </summary>
    public string KbId { get; set; }
    
    /// <summary>
    /// 文件名称
    /// </summary>
    public string FileName { get; set; } = "";

    /// <summary>
    /// 文件路径
    /// </summary>
    public string Url { get; set; } = "";
    
    /// <summary>
    /// 类型 file，url
    /// </summary>
    public string Type { get; set; }
    
    /// <summary>
    /// 文件大小
    /// </summary>
    public long Size { get; set; }
    
    /// <summary>
    /// 数据数量
    /// </summary>
    public int? DataCount { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public QuantizationState? Status { get; set; } = QuantizationState.Loading;

    /// <summary>
    /// 切分模式
    /// </summary>
    public SegmentPattern SegmentPattern { get; set; } = SegmentPattern.Subsection;

    public DateTime? Deleted { get; set; }
    public string? DeletedBy { get; set; }
}

public enum QuantizationState
{
    /// <summary>
    ///     默认
    /// </summary>
    Loading = 0,

    /// <summary>
    ///     完成
    /// </summary>
    Accomplish,

    /// <summary>
    ///     失败
    /// </summary>
    Fail
}

public enum SegmentPattern
{
    /// <summary>
    ///     分段
    /// </summary>
    Subsection,

    /// <summary>
    ///     问答
    /// </summary>
    QA
}