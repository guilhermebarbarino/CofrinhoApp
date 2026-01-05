using Cofrinho.Console.Domain.Entities;
using Cofrinho.Console.Domain.Enums;
using System.Text;

namespace Cofrinho.Console.Domain.Services;

public class CofrinhoService
{
    public Meta CriarMeta(string nome)
    {
        return new Meta(nome);
    }

    public void Depositar(Meta meta, decimal valor, string? descricao = null)
    {
        ValidarMeta(meta);

        var transacao = new Transacao(valor, TipoTransacao.Deposito, descricao);
        meta.AdicionarTransacao(transacao);
    }

    public void Sacar(Meta meta, decimal valor, string? descricao = null)
    {
        ValidarMeta(meta);

        if (valor <= 0)
            throw new ArgumentException("Valor deve ser maior que zero.");

        if (valor > meta.Saldo)
            throw new InvalidOperationException("Saldo insuficiente.");

        var transacao = new Transacao(valor, TipoTransacao.Saque, descricao);
        meta.AdicionarTransacao(transacao);
    }

    public string GerarExtrato(Meta meta)
    {
        ValidarMeta(meta);

        var sb = new StringBuilder();
        sb.AppendLine($"Meta: {meta.Nome}");
        sb.AppendLine(new string('-', 40));

        foreach (var t in meta.Transacoes.OrderBy(x => x.Data))
        {
            var sinal = t.Tipo == TipoTransacao.Deposito ? "+" : "-";
            sb.AppendLine($"{t.Data:dd/MM/yyyy HH:mm} | {t.Tipo} | {sinal}R$ {t.Valor:n2} | {t.Descricao}");
        }

        sb.AppendLine(new string('-', 40));
        sb.AppendLine($"Saldo atual: R$ {meta.Saldo:n2}");

        return sb.ToString();
    }

    private static void ValidarMeta(Meta meta)
    {
        if (meta is null)
            throw new ArgumentNullException(nameof(meta), "Meta não pode ser nula.");
    }
}
