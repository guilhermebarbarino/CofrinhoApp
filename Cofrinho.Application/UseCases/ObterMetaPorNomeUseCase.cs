using Cofrinho.Application.Interfaces;
using Cofrinho.Domain.Entities;
using Cofrinho.Domain.Interfaces;
using Cofrinho.Domain.Utils;

namespace Cofrinho.Application.Services.UseCases;

public sealed class ObterMetaPorNomeUseCase : IObterMetaPorNomeUseCase
{
    private readonly IMetaRepository _repo;

    public ObterMetaPorNomeUseCase(IMetaRepository repo)
    {
        _repo = repo;
    }

    public async Task<Meta> ExecuteAsync(string nome, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException(ErrorMessages.NomeMetaObrigatorio);

        var meta = _repo.GetByName(nome.Trim());

        if (meta is null)
            throw new KeyNotFoundException(ErrorMessages.MetaNaoEncontrada);

        return meta;
    }
}
