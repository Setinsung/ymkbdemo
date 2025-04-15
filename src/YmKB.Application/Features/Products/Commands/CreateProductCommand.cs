using Mediator;
using YmKB.Application.Contracts.Cache;
using YmKB.Application.Contracts.Persistence;
using YmKB.Application.Features.Products.DTOs;
using YmKB.Application.Features.Products.EventHandlers;
using YmKB.Application.Pipeline;
using YmKB.Domain.Entities.Trial;

namespace YmKB.Application.Features.Products.Commands;

/*
此文件定义用于在数据库中创建新产品的命令及其处理程序。
CreateProductCommand 封装了产品所需的数据，而
CreateProductCommandHandler 处理命令，创建一个 product 实体，
触发域事件（如 ProductCreatedEvent）并提交更改。这确保了
一种结构化且高效的产品创建方法。
 */

// Command 对象，该对象封装了创建新产品所需的数据。
// 它的字段直接映射到 ProductDto 的属性。
public record CreateProductCommand(
    string SKU,
    string Name,
    ProductCategoryDto? Category,
    string? Description,
    decimal Price,
    string? Currency,
    string? UOM
) : IFusionCacheRefreshRequest<ProductDto>, IRequiresValidation
{
    public IEnumerable<string>? Tags => ["products"];
}

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly IApplicationDbContext _context;

    public CreateProductCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<ProductDto> Handle(
        CreateProductCommand request,
        CancellationToken cancellationToken
    )
    {
        var product = new Product
        {
            SKU = request.SKU,
            Name = request.Name,
            Category = (ProductCategory)request.Category,
            Description = request.Description,
            Price = request.Price,
            Currency = request.Currency,
            UOM = request.UOM
        };

        product.AddDomainEvent(new ProductCreatedEvent(product));
        _context.Products.Add(product);
        await _context.SaveChangesAsync(cancellationToken);

        return new ProductDto()
        {
            Id = product.Id,
            Name = product.Name,
            SKU = product.SKU
        };
    }
}
