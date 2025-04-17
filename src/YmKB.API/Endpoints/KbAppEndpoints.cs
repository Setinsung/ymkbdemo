using Mediator;
using Microsoft.AspNetCore.Mvc;
using YmKB.Application.Features.KbApps.Commands;
using YmKB.Application.Features.KbApps.DTOs;
using YmKB.Application.Features.KbApps.Queries;
using YmKB.Application.Models;
using YmKB.Domain.Entities;

namespace YmKB.API.Endpoints;

public class KbAppEndpoins(ILogger<KbAppEndpoins> logger) : IEndpointRegistrar
{
    public void RegisterRoutes(IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/KbApps").WithTags("KbApps");

        group
            .MapGet(
                "/",
                async ([FromServices] IMediator mediator) =>
                {
                    var query = new GetAllKbAppsQuery();
                    return await mediator.Send(query);
                }
            )
            .Produces<IEnumerable<KbAppDto>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Get all KbApps")
            .WithDescription("Returns a list of all KbApps in the system.");

        group
            .MapGet(
                "/{id}",
                (IMediator mediator, [FromRoute] string id) =>
                    mediator.Send(new GetKbAppByIdQuery(id))
            )
            .Produces<KbAppDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Get KbApp by ID")
            .WithDescription("Returns the details of a specific KbApp by its unique ID.");

        group
            .MapPost(
                "/",
                ([FromServices] IMediator mediator, [FromBody] CreateKbAppCommand command) =>
                    mediator.Send(command)
            )
            .Produces<KbAppDto>(StatusCodes.Status201Created)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Create a new KbApp")
            .WithDescription("Creates a new KbApp with the provided details.");

        group
            .MapPut(
                "/",
                ([FromServices] IMediator mediator, [FromBody] UpdateKbAppCommand command) =>
                    mediator.Send(command)
            )
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Update an existing KbApp")
            .WithDescription("Updates the details of an existing KbApp.");

        group
            .MapDelete(
                "/",
                ([FromServices] IMediator mediator, [FromBody] DeleteKbAppCommand command) =>
                    mediator.Send(command)
            )
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Delete KbApps by IDs")
            .WithDescription("Deletes one or more KbApps by their unique IDs.");

        group
            .MapPost(
                "/pagination",
                (
                    [FromServices] IMediator mediator,
                    [FromBody] KbAppsWithPaginationQuery query
                ) => mediator.Send(query)
            )
            .Produces<PaginatedResult<KbAppDto>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Get KbApps with pagination")
            .WithDescription(
                "Returns a paginated list of KbApps based on search keywords, page size, and sorting options."
            );
    }
}
