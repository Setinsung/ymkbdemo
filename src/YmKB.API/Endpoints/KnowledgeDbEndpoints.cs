using Mediator;
using Microsoft.AspNetCore.Mvc;
using YmKB.Application.Features.KnowledgeDbs.Commands;
using YmKB.Application.Features.KnowledgeDbs.DTOs;
using YmKB.Application.Features.KnowledgeDbs.Queries;
using YmKB.Application.Models;
using YmKB.Domain.Entities;

namespace YmKB.API.Endpoints;

public class KnowledgeDbEndpoins(ILogger<KnowledgeDbEndpoins> logger) : IEndpointRegistrar
{
    public void RegisterRoutes(IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/KnowledgeDbs").WithTags("KnowledgeDbs");

        group
            .MapGet(
                "/",
                async ([FromServices] IMediator mediator) =>
                {
                    var query = new GetAllKnowledgeDbsQuery();
                    return await mediator.Send(query);
                }
            )
            .Produces<IEnumerable<KnowledgeDbDto>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Get all KnowledgeDbs")
            .WithDescription("Returns a list of all KnowledgeDbs in the system.");

        group
            .MapGet(
                "/{id}",
                (IMediator mediator, [FromRoute] string id) =>
                    mediator.Send(new GetKnowledgeDbByIdQuery(id))
            )
            .Produces<KnowledgeDbDto>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Get KnowledgeDb by ID")
            .WithDescription("Returns the details of a specific KnowledgeDb by its unique ID.");

        group
            .MapGet(
                "/SearchVectorTest",
                (
                    [FromServices] IMediator mediator,
                    string kbId,
                    string search,
                    double minRelevance = 0D
                ) => mediator.Send(new SearchVectorTestQuery(kbId, search, minRelevance)))
            .Produces<SearchedVectorsDto>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Search test KnowledgeDbs by keywords")
            .WithDescription("Returns a list of KnowledgeDbs matching the search keywords.");

        group
            .MapPost(
                "/",
                ([FromServices] IMediator mediator, [FromBody] CreateKnowledgeDbCommand command) =>
                    mediator.Send(command)
            )
            .Produces<KnowledgeDbDto>(StatusCodes.Status201Created)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Create a new KnowledgeDb")
            .WithDescription("Creates a new KnowledgeDb with the provided details.");

        group
            .MapPut(
                "/",
                ([FromServices] IMediator mediator, [FromBody] UpdateKnowledgeDbCommand command) =>
                    mediator.Send(command)
            )
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Update an existing KnowledgeDb")
            .WithDescription("Updates the details of an existing KnowledgeDb.");

        group
            .MapDelete(
                "/",
                ([FromServices] IMediator mediator, [FromBody] DeleteKnowledgeDbCommand command) =>
                    mediator.Send(command)
            )
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Delete KnowledgeDbs by IDs")
            .WithDescription("Deletes one or more KnowledgeDbs by their unique IDs.");

        group
            .MapPost(
                "/pagination",
                (
                    [FromServices] IMediator mediator,
                    [FromBody] KnowledgeDbsWithPaginationQuery query
                ) => mediator.Send(query)
            )
            .Produces<PaginatedResult<KnowledgeDbDto>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Get KnowledgeDbs with pagination")
            .WithDescription(
                "Returns a paginated list of KnowledgeDbs based on search keywords, page size, and sorting options."
            );
    }
}
