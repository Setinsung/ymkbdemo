using FluentValidation;
using YmKB.Application.Features.KbDocFiles.Commands;

namespace YmKB.Application.Features.KbDocFiles.Validators;

public class CreateKbDocFileCommandValidator : AbstractValidator<CreateKbDocFileCommand>
{
    public CreateKbDocFileCommandValidator()
    {
 
        RuleFor(command => command.FileName)
           .NotEmpty()
           .WithMessage("FileName is required.");
        RuleFor(command => command.Url)
           .NotEmpty()
           .WithMessage("Url is required.");
        //
    }
}

