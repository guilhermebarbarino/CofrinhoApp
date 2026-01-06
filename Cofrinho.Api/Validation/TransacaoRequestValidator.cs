using Cofrinho.Api.Contracts;
using Cofrinho.Domain.Utils;
using FluentValidation;

namespace Cofrinho.Api.Validation;

public sealed class TransacaoRequestValidator : AbstractValidator<TransacaoRequest>
{
    public TransacaoRequestValidator()
    {
        RuleFor(x => x.Valor)
            .GreaterThan(0).WithMessage(ErrorMessages.TransacaoValorMenorQueZero);

        RuleFor(x => x.Descricao)
            .MaximumLength(140).WithMessage("Descrição deve ter no máximo 140 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.Descricao));
    }
}
