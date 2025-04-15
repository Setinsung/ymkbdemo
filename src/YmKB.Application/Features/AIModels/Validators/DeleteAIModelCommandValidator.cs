using FluentValidation;
using YmKB.Application.Features.AIModels.Commands;

namespace YmKB.Application.Features.AIModels.Validators;

public class DeleteAIModelCommandValidator : AbstractValidator<DeleteAIModelCommand>
{
    public DeleteAIModelCommandValidator()
    {
        RuleFor(command => command.Ids)
            .NotEmpty()
            .WithMessage("At least one AIModel ID is required.")
            .Must(ids => ids != null && ids.All(id => !string.IsNullOrWhiteSpace(id)))
            .WithMessage("AIModel IDs must not be empty or whitespace.");
    }
}
