using FluentValidation;
using YmKB.Application.Features.KbDocFiles.Commands;

namespace YmKB.Application.Features.KbDocFiles.Validators;

public class UpdateKbDocFileCommandValidator : AbstractValidator<UpdateKbDocFileCommand>
{

    public UpdateKbDocFileCommandValidator()
    {
        RuleFor(command => command.Id).NotEmpty().WithMessage("KbDocFile ID is required.");
        RuleFor(command => command.FileName)
            .NotEmpty()
            .WithMessage("FileName is required.");
        RuleFor(command => command.Url)
            .NotEmpty()
            .WithMessage("Url is required.");
        //
    }
}
