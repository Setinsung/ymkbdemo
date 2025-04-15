using Mediator;
using Microsoft.AspNetCore.Mvc;
using YmKB.Application.Features.Products.Commands;
using YmKB.Application.Features.Products.DTOs;
using YmKB.Application.Features.Products.Queries;
using YmKB.Application.Models;

namespace YmKB.API.Endpoints;

public class ProductEndpoins(ILogger<ProductEndpoins> logger) : IEndpointRegistrar
{
    /// <summary>
    /// Registers the routes for product-related endpoints.
    /// </summary>
    /// <param name="routes">The route builder to which the endpoints will be added.</param>
    public void RegisterRoutes(IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/products").WithTags("products");

        /// <summary>
        /// Gets all products.
        /// </summary>
        /// <returns>A list of all products in the system.</returns>
        group.MapGet("/", async ([FromServices] IMediator mediator) =>
            {
                var query = new GetAllProductsQuery();
                return await mediator.Send(query);
            })
            .Produces<IEnumerable<ProductDto>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Get all products")
            .WithDescription("Returns a list of all products in the system.");

        /// <summary>
        /// Gets a product by its ID.
        /// </summary>
        /// <param name="id">The unique ID of the product.</param>
        /// <returns>The details of the specified product.</returns>
        group.MapGet("/{id}", (IMediator mediator, [FromRoute] string id) => mediator.Send(new GetProductByIdQuery(id)))
            .Produces<ProductDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Get product by ID")
            .WithDescription("Returns the details of a specific product by its unique ID.");

        /// <summary>
        /// Creates a new product.
        /// </summary>
        /// <param name="command">The command containing the details of the product to create.</param>
        /// <returns>The created product.</returns>
        group.MapPost("/",
                ([FromServices] IMediator mediator, [FromBody] CreateProductCommand command) => mediator.Send(command))
            .Produces<ProductDto>(StatusCodes.Status201Created)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Create a new product")
            .WithDescription("Creates a new product with the provided details.");

        /// <summary>
        /// Updates an existing product.
        /// </summary>
        /// <param name="command">The command containing the updated details of the product.</param>
        group.MapPut("/",
                ([FromServices] IMediator mediator, [FromBody] UpdateProductCommand command) => mediator.Send(command))
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Update an existing product")
            .WithDescription("Updates the details of an existing product.");

        /// <summary>
        /// Deletes products by their IDs.
        /// </summary>
        /// <param name="command">The command containing the IDs of the products to delete.</param>
        group.MapDelete("/",
                ([FromServices] IMediator mediator, [FromBody] DeleteProductCommand command) => mediator.Send(command))
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Delete products by IDs")
            .WithDescription("Deletes one or more products by their unique IDs.");

        /// <summary>
        /// Gets products with pagination and filtering.
        /// </summary>
        /// <param name="query">The query containing pagination and filtering parameters.</param>
        /// <returns>A paginated list of products.</returns>
        group.MapPost("/pagination",
                ([FromServices] IMediator mediator, [FromBody] ProductsWithPaginationQuery query) =>
                    mediator.Send(query))
            .Produces<PaginatedResult<ProductDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Get products with pagination")
            .WithDescription(
                "Returns a paginated list of products based on search keywords, page size, and sorting options.");
    }
}