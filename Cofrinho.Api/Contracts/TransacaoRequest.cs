using System.ComponentModel.DataAnnotations;

namespace Cofrinho.Api.Contracts
{
    public sealed class TransacaoRequest
    {
        public decimal Valor { get; set; }
        public string Descricao { get; set; }= string.Empty;
    }
}
