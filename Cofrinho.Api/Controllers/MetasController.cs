using Cofrinho.Api.Contracts;
using Cofrinho.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Cofrinho.Api.Controllers;

[ApiController]
[Route("api/metas")]
public class MetasController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Criar(
        [FromServices] ICriarMetaUseCase useCase,
        [FromBody] CreateMetaRequest request,
        CancellationToken ct)
    {
        await useCase.ExecuteAsync(request.Nome, ct);

        return CreatedAtAction(
            nameof(ObterPorNome),
            new { nome = request.Nome },
            new CreateMetaResponse("Meta criada com sucesso"));
    }

    [HttpGet("{nome}")]
    public async Task<ActionResult<MetaResponse>> ObterPorNome(
    [FromServices] IObterMetaPorNomeUseCase useCase,
    [FromRoute] string nome,
    CancellationToken ct)
    {
        var meta = await useCase.ExecuteAsync(nome, ct);
        return Ok(new MetaResponse(meta.Nome, meta.Saldo));
    }


    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromServices] IListarMetasUseCase useCase,
        CancellationToken ct)
    {
        var metas = await useCase.ExecuteAsync(ct);

        return Ok(metas.Select(m => new
        {
            m.Nome,
            m.Saldo,
            Transacoes = m.Transacoes.Count
        }));
    }

    [HttpPost("{nome}/deposito")]
    public async Task<IActionResult> Depositar(
        [FromServices] IDepositarUseCase useCase,
        [FromRoute] string nome,
        [FromBody] TransacaoRequest request,
        CancellationToken ct)
    {
        await useCase.ExecuteAsync(nome, request.Valor, request.Descricao, ct);
        return Ok(new TransacaoResponse("Depósito efetuado com sucesso!"));
    }

    [HttpPost("{nome}/saque")]
    public async Task<IActionResult> Sacar(
        [FromServices] ISacarUseCase useCase,
        [FromRoute] string nome,
        [FromBody] TransacaoRequest request,
        CancellationToken ct)
    {
        await useCase.ExecuteAsync(nome, request.Valor, request.Descricao, ct);
        return Ok(new TransacaoResponse("Saque realizado com sucesso!"));
    }

    [HttpGet("/api/extrato")]
    public async Task<IActionResult> Extrato(
        [FromServices] IGerarExtratoUseCase extratoUseCase,
        [FromServices] IGerarExtratoGeralUseCase extratoGeralUseCase,
        [FromQuery] string? nome,
        CancellationToken ct)
    {
        // Ideal: validar também esse "nome" (query) via DTO/Validator.
        if (!string.IsNullOrWhiteSpace(nome))
        {
            var texto = await extratoUseCase.ExecuteAsync(nome.Trim(), ct);
            return Ok(texto);
        }

        var geral = await extratoGeralUseCase.ExecuteAsync(ct);
        return Ok(geral);
    }
}
