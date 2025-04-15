using FluentValidation;
using YmKB.Application.Features.AIModels.Commands;

namespace YmKB.Application.Features.AIModels.Validators;

public class CreateAIModelCommandValidator : AbstractValidator<CreateAIModelCommand>
{
    public CreateAIModelCommandValidator()
    {
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
