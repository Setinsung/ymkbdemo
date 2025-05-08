using Mediator;
using Microsoft.AspNetCore.Mvc;
using YmKB.Application.Features.KbDocFiles.Commands;
using YmKB.Application.Features.KbDocFiles.DTOs;
using YmKB.Application.Features.KbDocFiles.Queries;
using YmKB.Application.Models;

namespace YmKB.API.Endpoints;

public class KbDocFileEndpoins(ILogger<KbDocFileEndpoins> logger) : IEndpointRegistrar
{
    public void RegisterRoutes(IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/KbDocFiles").WithTags("KbDocFiles");

        group
            .MapGet(
                "/",
                async ([FromServices] IMediator mediator) =>
                {
                    var query = new GetAllKbDocFilesQuery();
                    return await mediator.Send(query);
                }
            )
            .Produces<IEnumerable<KbDocFileDto>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Get all KbDocFiles")
            .WithDescription("Returns a list of all KbDocFiles in the system.");

        group
            .MapGet(
                "/{id}",
                (IMediator mediator, [FromRoute] string id) =>
                    mediator.Send(new GetKbDocFileByIdQuery(id))
            )
            .Produces<KbDocFileDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Get KbDocFile by ID")
            .WithDescription("Returns the details of a specific KbDocFile by its unique ID.");

        group
            .MapPost(
                "/",
                ([FromServices] IMediator mediator, [FromBody] CreateKbDocFileCommand command) =>
                    mediator.Send(command)
            )
            .Produces<KbDocFileDto>(StatusCodes.Status201Created)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Create a new KbDocFile")
            .WithDescription("Creates a new KbDocFile with the provided details.");

        group
            .MapPut(
                "/",
                ([FromServices] IMediator mediator, [FromBody] UpdateKbDocFileCommand command) =>
                    mediator.Send(command)
            )
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Update an existing KbDocFile")
            .WithDescription("Updates the details of an existing KbDocFile.");

        group
            .MapDelete(
                "/",
                ([FromServices] IMediator mediator, [FromBody] DeleteKbDocFileCommand command) =>
                    mediator.Send(command)
            )
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Delete KbDocFiles by IDs")
            .WithDescription("Deletes one or more KbDocFiles by their unique IDs.");

        group
            .MapPost(
                "/pagination",
                (
                    [FromServices] IMediator mediator,
                    [FromBody] KbDocFilesWithPaginationQuery query
                ) => mediator.Send(query)
            )
            .Produces<PaginatedResult<KbDocFileDto>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Get KbDocFiles with pagination")
            .WithDescription(
                "Returns a paginated list of KbDocFiles based on search keywords, page size, and sorting options."
            );
    }
}
