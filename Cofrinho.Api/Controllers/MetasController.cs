using Cofrinho.Api.Contracts;
using Cofrinho.Console.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Cofrinho.Api.Controllers;

[ApiController]
[Route("api/metas")]
public class MetasController : ControllerBase
{
    private readonly ICriarMetaUseCase _criarMeta;
    private readonly IListarMetasUseCase _listarMetas;
    private readonly IDepositarUseCase _depositar;
    private readonly ISacarUseCase _sacar;
    private readonly IGerarExtratoUseCase _extrato;

    public MetasController(
        ICriarMetaUseCase criarMeta,
        IListarMetasUseCase listarMetas,
        IDepositarUseCase depositar,
        ISacarUseCase sacar,
        IGerarExtratoUseCase extrato)
    {
        _criarMeta = criarMeta;
        _listarMetas = listarMetas;
        _depositar = depositar;
        _sacar = sacar;
        _extrato = extrato;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMetaRequest req, CancellationToken ct)
    {
        await _criarMeta.ExecuteAsync(req.Nome, ct);
        return CreatedAtAction(nameof(GetAll), null);
    }

    [HttpGet]
    public async Task<ActionResult<List<MetaResponse>>> GetAll(CancellationToken ct)
    {
        var metas = await _listarMetas.ExecuteAsync(ct);
        var result = metas
            .Select(m => new MetaResponse(m.Nome, m.Saldo, m.Transacoes.Count))
            .ToList();

        return Ok(result);
    }

    [HttpPost("{nome}/deposito")]
    public async Task<IActionResult> Depositar(string nome, [FromBody] TransacaoRequest req, CancellationToken ct)
    {
        await _depositar.ExecuteAsync(nome, req.Valor, req.Descricao, ct);
        return Ok();
    }

    [HttpPost("{nome}/saque")]
    public async Task<IActionResult> Sacar(string nome, [FromBody] TransacaoRequest req, CancellationToken ct)
    {
        await _sacar.ExecuteAsync(nome, req.Valor, req.Descricao, ct);
        return Ok();
    }

    [HttpGet("{nome}/extrato")]
    public async Task<ActionResult<string>> Extrato(string nome, CancellationToken ct)
    {
        var texto = await _extrato.ExecuteAsync(nome, ct);
        return Ok(texto);
    }
}
