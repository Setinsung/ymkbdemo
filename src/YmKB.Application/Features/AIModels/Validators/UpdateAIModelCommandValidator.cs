using FluentValidation;
using YmKB.Application.Features.AIModels.Commands;

namespace YmKB.Application.Features.AIModels.Validators;

public class UpdateAIModelCommandValidator : AbstractValidator<UpdateAIModelCommand>
{

    public UpdateAIModelCommandValidator()
    {
        RuleFor(command => command.Id).NotEmpty().WithMessage("AIModel ID is required.");
        RuleFor(command => command.Endpoint)
            .NotEmpty()
            .WithMessage("Endpoint is required.");
        RuleFor(command => command.ModelName)
            .NotEmpty()
            .WithMessage("ModelName is required.");
        RuleFor(command => command.ModelKey)
            .NotEmpty()
            .WithMessage("ModelKey is required.");
        RuleFor(command => command.ModelDescription)
            .NotEmpty()
            .WithMessage("ModelDescription is required.");
        RuleFor(command => command.AIModelType)
            .NotNull()
            .WithMessage("Product category is required.");
    }
}
