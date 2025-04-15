using Mediator;
using Microsoft.AspNetCore.Mvc;
using YmKB.Application.Features.Products.Commands;
using YmKB.Application.Features.Products.DTOs;
using YmKB.Application.Features.Products.Queries;
using YmKB.Application.Models;

namespace YmKB.API.Endpoints;

public class ProductEndpoins(ILogger<ProductEndpoins> logger) : IEndpointRegistrar
{
    public void RegisterRoutes(IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/products").WithTags("products").RequireAuthorization();

        group
            .MapGet(
                "/",
                async ([FromServices] IMediator mediator) =>
                {
                    var query = new GetAllProductsQuery();
                    return await mediator.Send(query);
                }
            )
            .Produces<IEnumerable<ProductDto>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Get all products")
            .WithDescription("Returns a list of all products in the system.");

        group
            .MapGet(
                "/{id}",
                (IMediator mediator, [FromRoute] string id) =>
                    mediator.Send(new GetProductByIdQuery(id))
            )
            .Produces<ProductDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Get product by ID")
            .WithDescription("Returns the details of a specific product by its unique ID.");

        group
            .MapPost(
                "/",
                ([FromServices] IMediator mediator, [FromBody] CreateProductCommand command) =>
                    mediator.Send(command)
            )
            .Produces<ProductDto>(StatusCodes.Status201Created)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Create a new product")
            .WithDescription("Creates a new product with the provided details.");

        group
            .MapPut(
                "/",
                ([FromServices] IMediator mediator, [FromBody] UpdateProductCommand command) =>
                    mediator.Send(command)
            )
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Update an existing product")
            .WithDescription("Updates the details of an existing product.");

        group
            .MapDelete(
                "/",
                ([FromServices] IMediator mediator, [FromBody] DeleteProductCommand command) =>
                    mediator.Send(command)
            )
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Delete products by IDs")
            .WithDescription("Deletes one or more products by their unique IDs.");

        group
            .MapPost(
                "/pagination",
                ([FromServices] IMediator mediator, [FromBody] ProductsWithPaginationQuery query) =>
                    mediator.Send(query)
            )
            .Produces<PaginatedResult<ProductDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Get products with pagination")
            .WithDescription(
                "Returns a paginated list of products based on search keywords, page size, and sorting options."
            );
    }
}
