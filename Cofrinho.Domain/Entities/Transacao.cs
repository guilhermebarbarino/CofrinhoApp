using Cofrinho.Domain.Enums;
using Cofrinho.Domain.Utils;

namespace Cofrinho.Domain.Entities;

public class Transacao
{
    public int Id { get; private set; }
    public DateTime Data { get; private set; }
    public decimal Valor { get; private set; }
    public TipoTransacao Tipo { get; private set; }
    public string Descricao { get; private set; } = "-";

 
    private Transacao() { }

    public Transacao(decimal valor, TipoTransacao tipo, string? descricao = null)
    {
        if (valor <= 0)
            throw new ArgumentException(ErrorMessages.TransacaoValorMenorQueZero);

        Data = DateTime.Now;
        Valor = valor;
        Tipo = tipo;
        Descricao = string.IsNullOrWhiteSpace(descricao) ? "-" : descricao.Trim();
    }
}