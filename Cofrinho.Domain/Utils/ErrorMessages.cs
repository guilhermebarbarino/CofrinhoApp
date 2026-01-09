using System.Diagnostics.CodeAnalysis;

namespace Cofrinho.Domain.Utils
{
    [ExcludeFromCodeCoverage]

    public static class ErrorMessages
    {
        public const string NomeMetaObrigatorio = "Nome da meta é obrigatório.";
        public const string MetaNaoEncontrada = "Meta não encontrada.";
        public const string TransacaoValorMenorQueZero = "Valor deve ser maior que zero.";
    }
}
