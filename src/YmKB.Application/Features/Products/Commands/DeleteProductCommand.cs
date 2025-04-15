// Summary:
// This file defines a command and its handler for deleting products from the database.
// The DeleteProductCommand encapsulates the product IDs to be deleted, while the
// DeleteProductCommandHandler processes the command, removes the corresponding products,
// triggers domain events such as ProductDeletedEvent, and commits the changes. This ensures
// a structured and efficient approach to handling product deletions.

using Mediator;
using Microsoft.Extensions.Logging;
using YmKB.Application.Contracts.Cache;
using YmKB.Application.Contracts.Persistence;
using YmKB.Application.Features.Products.EventHandlers;
using YmKB.Application.Pipeline;

namespace YmKB.Application.Features.Products.Commands;

// Command object that encapsulates the IDs of products to be deleted.
public record DeleteProductCommand(params IEnumerable<string> Ids)
    : IFusionCacheRefreshRequest<Unit>,
        IRequiresValidation
{
    public IEnumerable<string>? Tags => [ "products" ];
}

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand>
{
    private readonly IApplicationDbContext _dbContext;

    public DeleteProductCommandHandler(
        ILogger<DeleteProductCommandHandler> logger,
        IApplicationDbContext dbContext
    )
    {
        _dbContext = dbContext;
    }

    public async ValueTask<Unit> Handle(
        DeleteProductCommand request,
        CancellationToken cancellationToken
    )
    {
        var products = _dbContext.Products.Where(p => request.Ids.Contains(p.Id));

        foreach (var product in products)
        {
            product.AddDomainEvent(new ProductDeletedEvent(product));
            _dbContext.Products.Remove(product);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
