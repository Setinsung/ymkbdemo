using FluentValidation;
using YmKB.Application.Features.Products.Commands;

namespace YmKB.Application.Features.Products.Validators;

/// <summary>
/// Validator for UpdateProductCommand.
/// Uses FluentValidation to apply validation rules for updating product details.
/// </summary>
public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    /// <summary>
    /// Initializes validation rules for product update fields.
    /// </summary>
    public UpdateProductCommandValidator()
    {
        // Validate Product ID (required)
        RuleFor(command => command.Id).NotEmpty().WithMessage("Product ID is required.");

        // Validate SKU (required, max length 50)
        RuleFor(command => command.SKU)
            .NotEmpty()
            .WithMessage("SKU is required.")
            .MaximumLength(50)
            .WithMessage("SKU must not exceed 50 characters.");

        // Validate Name (required, max length 100)
        RuleFor(command => command.Name)
            .NotEmpty()
            .WithMessage("Product name is required.")
            .MaximumLength(100)
            .WithMessage("Product name must not exceed 100 characters.");

        // Validate Category (required)
        RuleFor(command => command.Category)
            .NotNull()
            .WithMessage("Product category is required.");

        // Validate Price (greater than 0)
        RuleFor(command => command.Price)
            .GreaterThan(0)
            .WithMessage("Price must be greater than 0.");

        // Validate Currency (required, valid currency code)
        RuleFor(command => command.Currency)
            .NotEmpty()
            .WithMessage("Currency is required.")
            .Must(BeAValidCurrency)
            .WithMessage("Invalid currency code.");

        // Validate Unit of Measure (UOM) (required, max length 10)
        RuleFor(command => command.UOM)
            .NotEmpty()
            .WithMessage("Unit of Measure (UOM) is required.")
            .MaximumLength(10)
            .WithMessage("UOM must not exceed 10 characters.");
    }

    /// <summary>
    /// Validates the provided currency code against a predefined list of valid codes.
    /// </summary>
    /// <param name="currency">The currency code to validate.</param>
    /// <returns>True if the currency is valid; otherwise, false.</returns>
    private bool BeAValidCurrency(string currency)
    {
        // Example list of valid currency codes
        var validCurrencies = new[] { "USD", "EUR", "GBP", "CNY" };
        return validCurrencies.Contains(currency);
    }
}
