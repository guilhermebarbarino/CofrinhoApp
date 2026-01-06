using Cofrinho.Application.Interfaces;
using Cofrinho.Domain.Entities;
using Cofrinho.Domain.Enums;
using Cofrinho.Domain.Interfaces;
using System.Text;

namespace Cofrinho.Application.Services;

public class CofrinhoAppService : ICofrinhoAppService
{
    private readonly IMetaRepository _repo;

    public CofrinhoAppService(IMetaRepository repo)
    {
        _repo = repo;
    }

    public Meta CriarMeta(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome da meta é obrigatório.");

        nome = nome.Trim();

        if (_repo.Exists(nome))
            throw new InvalidOperationException("Já existe uma meta com esse nome.");

        var meta = new Meta(nome);
        _repo.Add(meta);
        return meta;
    }

    public void Depositar(string nomeMeta, decimal valor, string? descricao = null)
    {
        var meta = ObterMetaObrigatoria(nomeMeta);
        meta.AdicionarTransacao(new Transacao(valor, TipoTransacao.Deposito, descricao));
    }

    public void Sacar(string nomeMeta, decimal valor, string? descricao = null)
    {
        var meta = ObterMetaObrigatoria(nomeMeta);

        if (valor <= 0)
            throw new ArgumentException("Valor deve ser maior que zero.");

        if (valor > meta.Saldo)
            throw new InvalidOperationException("Saldo insuficiente.");

        meta.AdicionarTransacao(new Transacao(valor, TipoTransacao.Saque, descricao));
    }

    public string GerarExtrato(string nomeMeta)
    {
        var meta = ObterMetaObrigatoria(nomeMeta);

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

    public IReadOnlyCollection<Meta> ListarMetas()
        => _repo.GetAll();

    private Meta ObterMetaObrigatoria(string nomeMeta)
    {
        if (string.IsNullOrWhiteSpace(nomeMeta))
            throw new ArgumentException("Nome da meta é obrigatório.");

        var meta = _repo.GetByName(nomeMeta.Trim());
        if (meta is null)
            throw new KeyNotFoundException("Meta não encontrada.");

        return meta;
    }
}
