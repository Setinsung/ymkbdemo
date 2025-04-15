using FluentValidation;
using YmKB.Application.Features.KnowledgeDbs.Commands;

namespace YmKB.Application.Features.KnowledgeDbs.Validators;

public class CreateKnowledgeDbCommandValidator : AbstractValidator<CreateKnowledgeDbCommand>
{
    public CreateKnowledgeDbCommandValidator()
    {
 
        RuleFor(command => command.Name)
           .NotEmpty()
           .WithMessage("Name is required.");
        RuleFor(command => command.Description)
           .NotEmpty()
           .WithMessage("Description is required.");
        //
    }
}

