using FluentValidation;
using YmKB.Application.Features.KbApps.Commands;

namespace YmKB.Application.Features.KbApps.Validators;

public class CreateKbAppCommandValidator : AbstractValidator<CreateKbAppCommand>
{
    public CreateKbAppCommandValidator()
    {
        RuleFor(command => command.Name).NotEmpty().WithMessage("Name is required.");
        RuleFor(command => command.Description).NotEmpty().WithMessage("Description is required.");
        RuleFor(command => command.Temperature)
            .InclusiveBetween(0, 100)
            .WithMessage("Temperature must be between 0 and 100.");
        RuleFor(command => command.Relevance)
            .InclusiveBetween(0, 100)
            .WithMessage("Relevance must be between 0 and 100.");
        RuleFor(command => command.MaxAskPromptSize)
            .GreaterThan(0)
            .WithMessage("MaxAskPromptSize must be greater than 0.");
        RuleFor(command => command.MaxMatchesCount)
            .GreaterThan(0)
            .WithMessage("MaxMatchesCount must be greater than 0.");
        RuleFor(command => command.AnswerTokens)
            .GreaterThan(0)
            .WithMessage("AnswerTokens must be greater than 0.");
    }
}
