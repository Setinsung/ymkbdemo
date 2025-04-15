using EntityFramework.Exceptions.Common;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace YmKB.API.ExceptionHandlers;

public class ProblemExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetailsService;

    public ProblemExceptionHandler(IProblemDetailsService problemDetailsService)
    {
        _problemDetailsService = problemDetailsService;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var problemDetails = exception switch
        {
            ValidationException ex => new HttpValidationProblemDetails
            {
                Status = StatusCodes.Status422UnprocessableEntity,
                Title = "Validation Error",
                Detail = "One or more validation errors occurred.",
                Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}",
                Errors = ex.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray()
                    )
            },
            UniqueConstraintException e => new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Unique Constraint Violation",
                Detail = e.ConstraintName != null && e.ConstraintProperties != null && e.ConstraintProperties.Any()
                        ? $"Unique constraint {e.ConstraintName} violated. Duplicate value for {e.ConstraintProperties[0]}."
                        : e.ConstraintName != null
                        ? $"Unique constraint {e.ConstraintName} violated."
                        : "A unique constraint violation occurred.",
                Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}",
            },
            CannotInsertNullException e => new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Null Value Error",
                Detail = "A required field was null.",
                Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}",
            },
            MaxLengthExceededException e => new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Max Length Exceeded",
                Detail = "A value exceeded the maximum allowed length.",
                Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}"
            },
            NumericOverflowException => new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Numeric Overflow",
                Detail = "A numeric value caused an overflow.",
                Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}",
            },
            ReferenceConstraintException e => new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Reference Constraint Violation",
                Detail = e.ConstraintName != null && e.ConstraintProperties != null && e.ConstraintProperties.Any()
                     ? $"Foreign key reference constraint {e.ConstraintName} violated. Involved columns: {string.Join(", ", e.ConstraintProperties)}."
                     : e.ConstraintName != null
                     ? $"Foreign key reference constraint {e.ConstraintName} violated."
                     : "A reference constraint violation occurred.",
                Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}",
            },
            DbUpdateException => new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Database Update Error",
                Detail = "An error occurred while updating the database.",
                Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}"
            },
            KeyNotFoundException ex => new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Resource Not Found",
                Detail = ex.Message,
                Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}"
            },
            InvalidOperationException ex => new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Invalid Operation",
                Detail = ex.Message,
                Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}"
            },
            _ => new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Internal Server Error",
                Detail = "An unexpected error occurred. Please try again later.",
                Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}"
            }
        };

        if (problemDetails is null)
        {
            // Return true to continue processing if the exception type is not handled.
            return true;
        }
        httpContext.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status400BadRequest;
        // Write ProblemDetails to the response
        return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problemDetails
        });
    }

}
