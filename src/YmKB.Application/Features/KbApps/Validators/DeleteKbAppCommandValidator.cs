using FluentValidation;
using YmKB.Application.Features.KbApps.Commands;

namespace YmKB.Application.Features.KbApps.Validators;

public class DeleteKbAppCommandValidator : AbstractValidator<DeleteKbAppCommand>
{
    public DeleteKbAppCommandValidator()
    {
        RuleFor(command => command.Ids)
            .NotEmpty()
            .WithMessage("At least one KbApp ID is required.")
            .Must(ids => ids != null && ids.All(id => !string.IsNullOrWhiteSpace(id)))
            .WithMessage("KbApp IDs must not be empty or whitespace.");    }
}
