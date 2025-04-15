// Summary:
// This file defines a command and its handler for updating product details in the database.
// The UpdateProductCommand encapsulates the necessary data to update a product, while the
// UpdateProductCommandHandler validates the product's existence, updates its details,
// triggers a domain event such as ProductUpdatedEvent, and commits the changes.

using Mediator;
using YmKB.Application.Contracts.Cache;
using YmKB.Application.Contracts.Persistence;
using YmKB.Application.Features.Products.DTOs;
using YmKB.Application.Features.Products.EventHandlers;
using YmKB.Application.Pipeline;
using YmKB.Domain.Entities.Trial;

namespace YmKB.Application.Features.Products.Commands;

// Command object that encapsulates the data required to update a product.
// Each field corresponds to a property in ProductDto.
public record UpdateProductCommand(
    string Id,
    string SKU,
    string Name,
    ProductCategoryDto? Category,
    string? Description,
    decimal Price,
    string? Currency,
    string? UOM
) : IFusionCacheRefreshRequest<Unit>, IRequiresValidation
{
    public IEnumerable<string>? Tags => ["products"];
}

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public UpdateProductCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<Unit> Handle(
        UpdateProductCommand request,
        CancellationToken cancellationToken
    )
    {
        var product = await _context
            .Products
            .FindAsync([request.Id], cancellationToken);
        if (product == null)
        {
            throw new KeyNotFoundException($"Product with Id '{request.Id}' was not found.");
        }

        product.SKU = request.SKU;
        product.Name = request.Name;
        product.Category = request.Category.HasValue
            ? (ProductCategory)request.Category
            : product.Category; // 如果未提供，保留现有类别。
        product.Description = request.Description;
        product.Price = request.Price;
        product.Currency = request.Currency;
        product.UOM = request.UOM;

        product.AddDomainEvent(new ProductUpdatedEvent(product));
        _context.Products.Update(product);

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
