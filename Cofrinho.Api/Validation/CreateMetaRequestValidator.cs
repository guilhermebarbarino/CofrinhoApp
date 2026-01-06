using Cofrinho.Api.Contracts;
using Cofrinho.Domain.Utils;
using FluentValidation;

namespace Cofrinho.Api.Validation;

public sealed class CreateMetaRequestValidator : AbstractValidator<CreateMetaRequest>
{
    public CreateMetaRequestValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage(ErrorMessages.NomeMetaObrigatorio)
            .MinimumLength(2).WithMessage("Nome da meta deve ter pelo menos 2 caracteres.")
            .MaximumLength(60).WithMessage("Nome da meta deve ter no máximo 60 caracteres.");
    }
}
