using Cofrinho.Application.Services;
using Cofrinho.Domain.Entities;
using Cofrinho.Domain.Interfaces;
using Moq;

namespace Cofrinho.Tests.Application;

public class CofrinhoAppServiceTests
{
    private readonly Mock<IMetaRepository> _repoMock;
    private readonly CofrinhoAppService _service;

    public CofrinhoAppServiceTests()
    {
        _repoMock = new Mock<IMetaRepository>();
        _service = new CofrinhoAppService(_repoMock.Object);
    }

    [Fact]
    public void CriarMeta_DeveCriarMeta_QuandoNomeValidoENaoExiste()
    {
        // Arrange
        _repoMock.Setup(r => r.Exists("Carro")).Returns(false);

        // Act
        var meta = _service.CriarMeta("Carro");

        // Assert
        Assert.NotNull(meta);
        Assert.Equal("Carro", meta.Nome);
        _repoMock.Verify(r => r.Add(It.IsAny<Meta>()), Times.Once);
    }

    [Fact]
    public void CriarMeta_DeveLancarExcecao_QuandoMetaJaExiste()
    {
        // Arrange
        _repoMock.Setup(r => r.Exists("Carro")).Returns(true);

        // Act + Assert
        var ex = Assert.Throws<InvalidOperationException>(() => _service.CriarMeta("Carro"));
        Assert.Contains("Já existe", ex.Message);
        _repoMock.Verify(r => r.Add(It.IsAny<Meta>()), Times.Never);
    }

    [Fact]
    public void Depositar_DeveAdicionarTransacao_QuandoMetaExisteEValorValido()
    {
        // Arrange
        var meta = new Meta("Casa");
        _repoMock.Setup(r => r.GetByName("Casa")).Returns(meta);

        // Act
        _service.Depositar("Casa", 100m, "primeiro deposito");

        // Assert
        Assert.Equal(100m, meta.Saldo);
        Assert.Single(meta.Transacoes);
    }

    [Fact]
    public void Sacar_DeveLancarExcecao_QuandoSaldoInsuficiente()
    {
        // Arrange
        var meta = new Meta("Viagem");
        _repoMock.Setup(r => r.GetByName("Viagem")).Returns(meta);

        // Act + Assert
        var ex = Assert.Throws<InvalidOperationException>(() => _service.Sacar("Viagem", 10m));
        Assert.Contains("Saldo insuficiente", ex.Message);
    }

    [Fact]
    public void GerarExtrato_DeveLancarExcecao_QuandoMetaNaoExiste()
    {
        // Arrange
        _repoMock.Setup(r => r.GetByName("Inexistente")).Returns((Meta?)null);

        // Act + Assert
        var ex = Assert.Throws<KeyNotFoundException>(() => _service.GerarExtrato("Inexistente"));
        Assert.Contains("Meta não encontrada", ex.Message);
    }
}
