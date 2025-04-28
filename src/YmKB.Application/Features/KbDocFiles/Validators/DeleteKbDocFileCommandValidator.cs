using FluentValidation;
using YmKB.Application.Features.KbDocFiles.Commands;

namespace YmKB.Application.Features.KbDocFiles.Validators;

public class DeleteKbDocFileCommandValidator : AbstractValidator<DeleteKbDocFileCommand>
{
    public DeleteKbDocFileCommandValidator()
    {
        RuleFor(command => command.Ids)
            .NotEmpty()
            .WithMessage("At least one KbDocFile ID is required.")
            .Must(ids => ids != null && ids.All(id => !string.IsNullOrWhiteSpace(id)))
            .WithMessage("KbDocFile IDs must not be empty or whitespace.");
    }
}
