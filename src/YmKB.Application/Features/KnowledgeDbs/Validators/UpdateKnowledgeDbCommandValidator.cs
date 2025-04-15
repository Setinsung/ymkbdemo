using FluentValidation;
using YmKB.Application.Features.KnowledgeDbs.Commands;

namespace YmKB.Application.Features.KnowledgeDbs.Validators;

public class UpdateKnowledgeDbCommandValidator : AbstractValidator<UpdateKnowledgeDbCommand>
{

    public UpdateKnowledgeDbCommandValidator()
    {
        RuleFor(command => command.Id).NotEmpty().WithMessage("KnowledgeDb ID is required.");
        RuleFor(command => command.Name)
            .NotEmpty()
            .WithMessage("Name is required.");
        RuleFor(command => command.Description)
            .NotEmpty()
            .WithMessage("Description is required.");
        //
    }
}
