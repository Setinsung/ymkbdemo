using YmKB.Domain.Abstractions.Common;

namespace YmKB.Domain.Entities;

/// <summary>
/// 量化对列
/// </summary>
public class QuantizedList : BaseAuditableEntity
{
    /// <summary>
    /// 知识库Id
    /// </summary>
    public string KbId { get; set; }

    /// <summary>
    /// 量化文档Id
    /// </summary>
    public string KbDocFileId { get; set; }

    /// <summary>
    /// 处理状态
    /// </summary>
    public QuantizedListState Status { get; set; } = QuantizedListState.Pending;

    /// <summary>
    /// 处理结果备注
    /// </summary>
    public string Remark { get; set; }

    /// <summary>
    /// 处理时间
    /// </summary>
    public DateTime? ProcessTime { get; set; }
}

/// <summary>
/// 表示量化列表的状态。
/// </summary>
public enum QuantizedListState
{
    /// <summary>
    /// 待处理状态。
    /// </summary>
    Pending = 1,
    
    /// <summary>
    /// 处理中状态。
    /// </summary>
    Processing = 2,

    /// <summary>
    /// 成功状态。
    /// </summary>
    Success = 3,

    /// <summary>
    /// 失败状态。
    /// </summary>
    Fail = 4
}