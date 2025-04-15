using YmKB.Domain.Abstractions.Common;

namespace YmKB.Domain.Entities.Trial;

public class Product : BaseAuditableEntity, IAuditTrial
{
    /// <summary>
    /// 获取或设置产品的 SKU。
    /// </summary>
    public string SKU { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置产品的名称。
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置产品的类别。
    /// </summary>
    public ProductCategory Category { get; set; } = ProductCategory.Electronics;

    /// <summary>
    /// 获取或设置产品的描述。
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 获取或设置产品的价格。
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// 获取或设置产品价格的货币。
    /// </summary>
    public string? Currency { get; set; }

    /// <summary>
    /// 获取或设置产品的度量单位。
    /// </summary>
    public string? UOM { get; set; }
}


/// <summary>
/// Represents the category of a product.
/// </summary>
public enum ProductCategory
{
    /// <summary>
    /// Electronics category.
    /// </summary>
    Electronics,

    /// <summary>
    /// Furniture category.
    /// </summary>
    Furniture,

    /// <summary>
    /// Clothing category.
    /// </summary>
    Clothing,

    /// <summary>
    /// Food category.
    /// </summary>
    Food,

    /// <summary>
    /// Beverages category.
    /// </summary>
    Beverages,

    /// <summary>
    /// Health care category.
    /// </summary>
    HealthCare,

    /// <summary>
    /// Sports category.
    /// </summary>
    Sports,
}