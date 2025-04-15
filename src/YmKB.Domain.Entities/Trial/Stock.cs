using YmKB.Domain.Abstractions.Common;

namespace YmKB.Domain.Entities.Trial;

public class Stock : BaseAuditableEntity, IAuditTrial, ISoftDelete
{
    /// <summary>
    /// 获取或设置产品 ID。
    /// </summary>
    public string? ProductId { get; set; }

    /// <summary>
    /// 获取或设置与存储关联的产品。
    /// </summary>
    public Product? Product { get; set; }

    /// <summary>
    /// 获取或设置存储的数量。
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// 获取或设置存储的位置。
    /// </summary>
    public string Location { get; set; } = string.Empty;

    public DateTime? Deleted { get; set; }
    public string? DeletedBy { get; set; }
}