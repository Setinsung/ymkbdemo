// 此文件定义产品的数据传输对象 （DTO） 和产品类别的枚举。
// ProductDto 类封装了应用程序层之间数据传输的产品详细信息。
// ProductCategoryDto 枚举为产品提供预定义的类别。

namespace YmKB.Application.Features.Products.DTOs;

// 一个 DTO 表示一个产品，用于在应用程序层之间传输数据。
// 默认情况下，字段名称与相应的实体字段匹配。对于枚举或引用的实体，使用 Dto 后缀。
public class ProductDto
{
    public string Id { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public ProductCategoryDto? Category { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? Currency { get; set; }
    public string? UOM { get; set; }
}

// 表示可能的产品类别的枚举。
public enum ProductCategoryDto
{
    Electronics,
    Furniture,
    Clothing,
    Food,
    Beverages,
    HealthCare,
    Sports
}
