using Mediator;
using Microsoft.AspNetCore.Mvc;
using YmKB.Application.Features.QuantizedLists.Commands;
using YmKB.Application.Features.QuantizedLists.DTOs;
using YmKB.Application.Features.QuantizedLists.Queries;
using YmKB.Application.Models;

namespace YmKB.API.Endpoints;

public class QuantizedListEndpoins(ILogger<QuantizedListEndpoins> logger) : IEndpointRegistrar
{
    public void RegisterRoutes(IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/QuantizedLists").WithTags("QuantizedLists");

        group
            .MapGet(
                "/",
                async ([FromServices] IMediator mediator) =>
                {
                    var query = new GetAllQuantizedListsQuery();
                    return await mediator.Send(query);
                }
            )
            .Produces<IEnumerable<QuantizedListDto>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Get all QuantizedLists")
            .WithDescription("Returns a list of all QuantizedLists in the system.");

        group
            .MapGet(
                "/{id}",
                (IMediator mediator, [FromRoute] string id) =>
                    mediator.Send(new GetQuantizedListByIdQuery(id))
            )
            .Produces<QuantizedListDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Get QuantizedList by ID")
            .WithDescription("Returns the details of a specific QuantizedList by its unique ID.");

        group
            .MapPost(
                "/",
                (
                    [FromServices] IMediator mediator,
                    [FromBody] CreateQuantizedListCommand command
                ) => mediator.Send(command)
            )
            .Produces<QuantizedListDto>(StatusCodes.Status201Created)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Create a new QuantizedList")
            .WithDescription("Creates a new QuantizedList with the provided details.");

        group
            .MapPut(
                "/",
                (
                    [FromServices] IMediator mediator,
                    [FromBody] UpdateQuantizedListCommand command
                ) => mediator.Send(command)
            )
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Update an existing QuantizedList")
            .WithDescription("Updates the details of an existing QuantizedList.");

        group
            .MapDelete(
                "/",
                (
                    [FromServices] IMediator mediator,
                    [FromBody] DeleteQuantizedListCommand command
                ) => mediator.Send(command)
            )
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Delete QuantizedLists by IDs")
            .WithDescription("Deletes one or more QuantizedLists by their unique IDs.");

        group
            .MapPost(
                "/pagination",
                (
                    [FromServices] IMediator mediator,
                    [FromBody] QuantizedListsWithPaginationQuery query
                ) => mediator.Send(query)
            )
            .Produces<PaginatedResult<QuantizedListDto>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Get QuantizedLists with pagination")
            .WithDescription(
                "Returns a paginated list of QuantizedLists based on search keywords, page size, and sorting options."
            );
    }
}
