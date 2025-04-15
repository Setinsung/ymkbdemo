using FluentValidation;
using YmKB.Application.Features.Products.Commands;

namespace YmKB.Application.Features.Products.Validators;

/// <summary>
/// Validator for DeleteProductCommand.
/// Uses FluentValidation to apply validation rules for deleting products.
/// </summary>
public class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
{
    /// <summary>
    /// Initializes validation rules for the IDs of products to be deleted.
    /// </summary>
    public DeleteProductCommandValidator()
    {
        // Validate that the IDs collection is not empty
        RuleFor(command => command.Ids)
            .NotEmpty()
            .WithMessage("At least one product ID is required.")
            .Must(ids => ids != null && ids.All(id => !string.IsNullOrWhiteSpace(id)))
            .WithMessage("Product IDs must not be empty or whitespace.");
    }
}
