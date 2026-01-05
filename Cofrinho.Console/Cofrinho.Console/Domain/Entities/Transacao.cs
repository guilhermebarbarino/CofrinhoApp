using Cofrinho.Console.Domain.Enums;

namespace Cofrinho.Console.Domain.Entities;

public class Transacao
{
    public DateTime Data { get; }
    public decimal Valor { get; }
    public TipoTransacao Tipo { get; }
    public string Descricao { get; }

    public Transacao(decimal valor, TipoTransacao tipo, string? descricao = null)
    {
        if (valor <= 0)
            throw new ArgumentException("Valor deve ser maior que zero.");

        Data = DateTime.Now;
        Valor = valor;
        Tipo = tipo;
        Descricao = string.IsNullOrWhiteSpace(descricao) ? "-" : descricao.Trim();
    }
}
