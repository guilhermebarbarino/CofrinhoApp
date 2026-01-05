namespace Cofrinho.Console.Domain.Entities;

public class Meta
{
    public string Nome { get; }
    private readonly List<Transacao> _transacoes = new();

    public IReadOnlyCollection<Transacao> Transacoes => _transacoes.AsReadOnly();

    public decimal Saldo => _transacoes.Sum(t => t.Tipo == Enums.TipoTransacao.Deposito ? t.Valor : -t.Valor);

    public Meta(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome da meta é obrigatório.");

        Nome = nome.Trim();
    }

    public void AdicionarTransacao(Transacao transacao)
    {
        _transacoes.Add(transacao);
    }
}
