using FluentValidation;
using YmKB.Application.Features.KnowledgeDbs.Commands;

namespace YmKB.Application.Features.KnowledgeDbs.Validators;

public class DeleteKnowledgeDbCommandValidator : AbstractValidator<DeleteKnowledgeDbCommand>
{
    public DeleteKnowledgeDbCommandValidator()
    {
        RuleFor(command => command.Ids)
            .NotEmpty()
            .WithMessage("At least one KnowledgeDb ID is required.")
            .Must(ids => ids != null && ids.All(id => !string.IsNullOrWhiteSpace(id)))
            .WithMessage("KnowledgeDb IDs must not be empty or whitespace.");
    }
}
