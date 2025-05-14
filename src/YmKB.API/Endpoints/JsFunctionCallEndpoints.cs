using Mediator;
using Microsoft.AspNetCore.Mvc;
using YmKB.Application.Features.JsFunctionCalls.Commands;
using YmKB.Application.Features.JsFunctionCalls.DTOs;
using YmKB.Application.Features.JsFunctionCalls.Queries;
using YmKB.Application.Models;
using YmKB.Domain.Entities;

namespace YmKB.API.Endpoints;

public class JsFunctionCallEndpoins(ILogger<JsFunctionCallEndpoins> logger) : IEndpointRegistrar
{
    public void RegisterRoutes(IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/JsFunctionCalls").WithTags("JsFunctionCalls");

        group
            .MapGet(
                "/",
                async ([FromServices] IMediator mediator) =>
                {
                    var query = new GetAllJsFunctionCallsQuery();
                    return await mediator.Send(query);
                }
            )
            .Produces<IEnumerable<JsFunctionCallDto>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Get all JsFunctionCalls")
            .WithDescription("Returns a list of all JsFunctionCalls in the system.");

        group
            .MapGet(
                "/{id}",
                (IMediator mediator, [FromRoute] string id) =>
                    mediator.Send(new GetJsFunctionCallByIdQuery(id))
            )
            .Produces<JsFunctionCallDto>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Get JsFunctionCall by ID")
            .WithDescription("Returns the details of a specific JsFunctionCall by its unique ID.");

        group
            .MapPost(
                "/",
                ([FromServices] IMediator mediator, [FromBody] CreateJsFunctionCallCommand command) =>
                    mediator.Send(command)
            )
            .Produces<JsFunctionCallDto>(StatusCodes.Status201Created)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Create a new JsFunctionCall")
            .WithDescription("Creates a new JsFunctionCall with the provided details.");

        group
            .MapPut(
                "/",
                ([FromServices] IMediator mediator, [FromBody] UpdateJsFunctionCallCommand command) =>
                    mediator.Send(command)
            )
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Update an existing JsFunctionCall")
            .WithDescription("Updates the details of an existing JsFunctionCall.");

        group
            .MapDelete(
                "/",
                ([FromServices] IMediator mediator, [FromBody] DeleteJsFunctionCallCommand command) =>
                    mediator.Send(command)
            )
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Delete JsFunctionCalls by IDs")
            .WithDescription("Deletes one or more JsFunctionCalls by their unique IDs.");

        group
            .MapPost(
                "/pagination",
                (
                    [FromServices] IMediator mediator,
                    [FromBody] JsFunctionCallsWithPaginationQuery query
                ) => mediator.Send(query)
            )
            .Produces<PaginatedResult<JsFunctionCallDto>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Get JsFunctionCalls with pagination")
            .WithDescription(
                "Returns a paginated list of JsFunctionCalls based on search keywords, page size, and sorting options."
            );
    }
}
