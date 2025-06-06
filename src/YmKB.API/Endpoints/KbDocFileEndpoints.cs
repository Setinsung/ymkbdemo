﻿using Mediator;
using Microsoft.AspNetCore.Mvc;
using YmKB.API.BackGroundServices;
using YmKB.Application.Features.KbDocFiles.Commands;
using YmKB.Application.Features.KbDocFiles.DTOs;
using YmKB.Application.Features.KbDocFiles.Queries;
using YmKB.Application.Models;
using YmKB.Domain.Entities;

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
            .MapGet(
                "/vectors",
                (
                    [FromServices] IMediator mediator,
                    string kbDocFileId,
                    int pageNumber,
                    int pageSize
                ) => mediator.Send(new KbDocFileVectorsQuery(kbDocFileId, pageNumber, pageSize))
            )
            .Produces<PaginatedResult<KbDocFileVectorDto>>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Get KbDocFileVectors by KbDocFileId")
            .WithDescription(
                "Returns the details of a specific KbDocFileVectors by its unique ID."
            );

        group
            .MapPost(
                "/",
                async (
                    [FromServices] IMediator mediator,
                    [FromBody] CreateKbDocFileCommand command
                ) =>
                {
                    var res = await mediator.Send(command);
                    await QuantitativeBackgroundService.AddKbDocFileAsync(res);
                    return res;
                }
            )
            .Produces<KbDocFileDto>(StatusCodes.Status201Created)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Create a new KbDocFile")
            .WithDescription("Creates a new KbDocFile with the provided details.");

        group
            .MapPost(
                "/retryQuantization/{id}",
                async ([FromServices] IMediator mediator, [FromRoute] string id) =>
                {
                    var kbDocFileDto = await mediator.Send(new GetKbDocFileByIdQuery(id));
                    if (kbDocFileDto == null)
                        throw new KeyNotFoundException("文档不存在");
                    if (kbDocFileDto.Status == QuantizationState.Accomplish)
                        return;
                    await QuantitativeBackgroundService.AddKbDocFileAsync(kbDocFileDto.Id);
                }
            )
            .Produces(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Retry Quantization")
            .WithDescription("Retry Quantization");

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
            .MapDelete(
                "/vectors",
                (
                    [FromServices] IMediator mediator,
                    [FromBody] DeleteKbDocFileVectorCommand command
                ) => mediator.Send(command)
            )
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Delete KbDocFileVectors by DocumentId")
            .WithDescription("Delete vector in vector database by DocumentId.");

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
