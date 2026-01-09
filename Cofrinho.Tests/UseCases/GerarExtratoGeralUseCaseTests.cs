using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cofrinho.Console.Application.Services.UseCases;
using Cofrinho.Domain.Entities;
using Cofrinho.Domain.Enums;
using Cofrinho.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

public class GerarExtratoGeralUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_Deve_Retornar_Mensagem_Se_Nao_Existir_Meta()
    {
        // Arrange
        var repo = new Mock<IMetaRepository>();
        repo.Setup(r => r.GetAll()).Returns(new List<Meta>());

        var useCase = new GerarExtratoGeralUseCase(repo.Object);

        // Act
        var resultado = await useCase.ExecuteAsync(CancellationToken.None);

        // Assert
        resultado.Should().Be("Nenhuma meta cadastrada.");
        repo.Verify(r => r.GetAll(), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_Deve_Gerar_Extrato_Com_Meta_Sem_Transacoes()
    {
        // Arrange
        var meta = new Meta("Carro");

        var repo = new Mock<IMetaRepository>();
        repo.Setup(r => r.GetAll()).Returns(new List<Meta> { meta });

        var useCase = new GerarExtratoGeralUseCase(repo.Object);

        // Act
        var resultado = await useCase.ExecuteAsync(CancellationToken.None);

        // Assert
        resultado.Should().Contain("=== EXTRATO GERAL ===");
        resultado.Should().Contain("Carro");
        resultado.Should().Contain("Saldo: R$ 0,00");
        resultado.Should().Contain("Sem transações.");

        repo.Verify(r => r.GetAll(), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_Deve_Gerar_Extrato_Com_Transacoes_Ordenadas_Por_Data()
    {
        // Arrange
        var meta = new Meta("Viagem");

        meta.AdicionarTransacao(new Transacao(200, TipoTransacao.Deposito, "Inicial"));
        meta.AdicionarTransacao(new Transacao(50, TipoTransacao.Saque, "Hotel"));

        var repo = new Mock<IMetaRepository>();
        repo.Setup(r => r.GetAll()).Returns(new List<Meta> { meta });

        var useCase = new GerarExtratoGeralUseCase(repo.Object);

        // Act
        var resultado = await useCase.ExecuteAsync(CancellationToken.None);

        // Assert
        resultado.Should().Contain("--- Viagem ---");
        resultado.Should().Contain("DEP");
        resultado.Should().Contain("SAQ");
        resultado.Should().Contain("+R$ 200,00");
        resultado.Should().Contain("-R$ 50,00");

        repo.Verify(r => r.GetAll(), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_Deve_Ordenar_Metas_Por_Nome()
    {
        // Arrange
        var metaB = new Meta("Viagem");
        var metaA = new Meta("Carro");

        var repo = new Mock<IMetaRepository>();
        repo.Setup(r => r.GetAll()).Returns(new List<Meta> { metaB, metaA });

        var useCase = new GerarExtratoGeralUseCase(repo.Object);

        // Act
        var resultado = await useCase.ExecuteAsync(CancellationToken.None);

        // Assert
        var indexCarro = resultado.IndexOf("Carro");
        var indexViagem = resultado.IndexOf("Viagem");

        indexCarro.Should().BeLessThan(indexViagem);
    }

    [Fact]
    public async Task ExecuteAsync_Deve_Retornar_String_Com_Conteudo_Completo()
    {
        // Arrange
        var meta = new Meta("Casa");
        meta.AdicionarTransacao(new Transacao(1000, TipoTransacao.Deposito, "Entrada"));

        var repo = new Mock<IMetaRepository>();
        repo.Setup(r => r.GetAll()).Returns(new List<Meta> { meta });

        var useCase = new GerarExtratoGeralUseCase(repo.Object);

        // Act
        var resultado = await useCase.ExecuteAsync(CancellationToken.None);

        // Assert
        resultado.Should().Contain("=== EXTRATO GERAL ===");
        resultado.Should().Contain("Gerado em:");
        resultado.Should().Contain("Casa");
        resultado.Should().Contain("DEP");
        resultado.Should().Contain("Entrada");
    }
}
