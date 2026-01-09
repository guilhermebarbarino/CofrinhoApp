using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cofrinho.Application.UseCases;
using Cofrinho.Domain.Entities;
using Cofrinho.Domain.Enums;
using Cofrinho.Domain.Interfaces;
using Cofrinho.Domain.Utils;
using FluentAssertions;
using Moq;
using Xunit;

public class GerarExtratoUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_Deve_Gerar_Extrato_Da_Meta_Com_Header_Linha_E_Saldo()
    {
        // Arrange
        var meta = new Meta("Carro");
        meta.AdicionarTransacao(new Transacao(100m, TipoTransacao.Deposito, "Pix"));
        meta.AdicionarTransacao(new Transacao(40m, TipoTransacao.Saque, "Lavagem"));

        var repo = new Mock<IMetaRepository>();
        repo.Setup(r => r.GetByName("Carro")).Returns(meta);

        var useCase = new GerarExtratoUseCase(repo.Object);

        // Act
        var resultado = await useCase.ExecuteAsync("Carro", CancellationToken.None);

        // Assert
        resultado.Should().Contain("Meta: Carro");
        resultado.Should().Contain(new string('-', 40));
        resultado.Should().Contain("Saldo atual: R$");

        resultado.Should().Contain("+R$ 100,00");
        resultado.Should().Contain("-R$ 40,00");
        resultado.Should().Contain("Pix");
        resultado.Should().Contain("Lavagem");

        repo.Verify(r => r.GetByName("Carro"), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_Deve_Lancar_KeyNotFound_Se_Meta_Nao_Existe()
    {
        // Arrange
        var repo = new Mock<IMetaRepository>();
        repo.Setup(r => r.GetByName("inexistente")).Returns((Meta?)null);

        var useCase = new GerarExtratoUseCase(repo.Object);

        // Act
        Func<Task> act = () => useCase.ExecuteAsync("inexistente", CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<KeyNotFoundException>()
            .WithMessage($"*{ErrorMessages.MetaNaoEncontrada}*");

        repo.Verify(r => r.GetByName("inexistente"), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_Deve_Chamar_GetByName_Com_Nome_Trimado()
    {
        // Arrange
        var meta = new Meta("Carro");

        var repo = new Mock<IMetaRepository>();
        repo.Setup(r => r.GetByName("Carro")).Returns(meta);

        var useCase = new GerarExtratoUseCase(repo.Object);

        // Act
        await useCase.ExecuteAsync("   Carro   ", CancellationToken.None);

        // Assert
        repo.Verify(r => r.GetByName("Carro"), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_Deve_Ordenar_Transacoes_Por_Data_No_Extrato()
    {
        // Arrange
        var meta = new Meta("Viagem");

        meta.AdicionarTransacao(new Transacao(200m, TipoTransacao.Deposito, "A"));
        meta.AdicionarTransacao(new Transacao(50m, TipoTransacao.Saque, "B"));

        // Se sua entidade Transacao define Data internamente com DateTime.Now,
        // a ordem pode depender do instante de criação. Esse teste valida pela posição no texto.
        var repo = new Mock<IMetaRepository>();
        repo.Setup(r => r.GetByName("Viagem")).Returns(meta);

        var useCase = new GerarExtratoUseCase(repo.Object);

        // Act
        var resultado = await useCase.ExecuteAsync("Viagem", CancellationToken.None);

        // Assert
        var idxA = resultado.IndexOf("| Deposito |", StringComparison.Ordinal);
        var idxB = resultado.IndexOf("| Saque |", StringComparison.Ordinal);

        idxA.Should().BeGreaterThan(-1);
        idxB.Should().BeGreaterThan(-1);

        // Como as transações são adicionadas em sequência, normalmente DEP vem antes de SAQ no texto,
        // e a ordenação por Data deve manter isso.
        idxA.Should().BeLessThan(idxB);
    }

    [Fact]
    public async Task ExecuteAsync_Deve_Exibir_Sinal_Correto_Para_Deposito_E_Saque()
    {
        // Arrange
        var meta = new Meta("Casa");
        meta.AdicionarTransacao(new Transacao(10m, TipoTransacao.Deposito, "D"));
        meta.AdicionarTransacao(new Transacao(5m, TipoTransacao.Saque, "S"));

        var repo = new Mock<IMetaRepository>();
        repo.Setup(r => r.GetByName("Casa")).Returns(meta);

        var useCase = new GerarExtratoUseCase(repo.Object);

        // Act
        var resultado = await useCase.ExecuteAsync("Casa", CancellationToken.None);

        // Assert
        resultado.Should().Contain("+R$ 10,00");
        resultado.Should().Contain("-R$ 5,00");
    }
}
