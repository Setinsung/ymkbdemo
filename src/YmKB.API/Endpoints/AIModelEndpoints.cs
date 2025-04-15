using Mediator;
using Microsoft.AspNetCore.Mvc;
using YmKB.Application.Features.AIModels.Commands;
using YmKB.Application.Features.AIModels.DTOs;
using YmKB.Application.Features.AIModels.Queries;
using YmKB.Domain.Entities;

namespace YmKB.API.Endpoints;

public class AIModelEndpoins(ILogger<AIModelEndpoins> logger) : IEndpointRegistrar
{
    public void RegisterRoutes(IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/AIModels").WithTags("AIModels");

        group
            .MapGet(
                "/",
                async ([FromServices] IMediator mediator) =>
                {
                    var query = new GetAllAIModelsQuery();
                    return await mediator.Send(query);
                }
            )
            .Produces<IEnumerable<AIModelDto>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Get all AIModels")
            .WithDescription("Returns a list of all AIModels in the system.");

        group
            .MapGet(
                "/search",
                async (
                    [FromServices] IMediator mediator,
                    [FromQuery] string? searchKeyword,
                    [FromQuery] AIModelType? aiModelType
                ) => await mediator.Send(new SearchAIModelsQuery(searchKeyword, aiModelType))
            )
            .Produces<IEnumerable<AIModelDto>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Search AIModels")
            .WithDescription("Returns a list of all AIModels in the system.");

        group
            .MapGet(
                "/{id}",
                (IMediator mediator, [FromRoute] string id) =>
                    mediator.Send(new GetAIModelByIdQuery(id))
            )
            .Produces<AIModelDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Get AIModel by ID")
            .WithDescription("Returns the details of a specific AIModel by its unique ID.");

        group
            .MapPost(
                "/",
                ([FromServices] IMediator mediator, [FromBody] CreateAIModelCommand command) =>
                    mediator.Send(command)
            )
            .Produces<AIModelDto>(StatusCodes.Status201Created)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Create a new AIModel")
            .WithDescription("Creates a new AIModel with the provided details.");

        group
            .MapPut(
                "/",
                ([FromServices] IMediator mediator, [FromBody] UpdateAIModelCommand command) =>
                    mediator.Send(command)
            )
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Update an existing AIModel")
            .WithDescription("Updates the details of an existing AIModel.");

        group
            .MapDelete(
                "/",
                ([FromServices] IMediator mediator, [FromBody] DeleteAIModelCommand command) =>
                    mediator.Send(command)
            )
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Delete AIModels by IDs")
            .WithDescription("Deletes one or more AIModels by their unique IDs.");
    }
}
