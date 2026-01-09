using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cofrinho.Api.Controllers;
using Cofrinho.Api.Contracts;
using Cofrinho.Application.Interfaces;
using Cofrinho.Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

public class MetasControllerTests
{
    [Fact]
    public async Task Criar_Deve_Retornar_CreatedAtAction()
    {
        // Arrange
        var useCase = new Mock<ICriarMetaUseCase>();
        useCase.Setup(x => x.ExecuteAsync("Carro", It.IsAny<CancellationToken>()))
               .Returns(Task.CompletedTask);

        var controller = new MetasController();
        var request = new CreateMetaRequest();
        request.Nome = "Carro";

        // Act
        var result = await controller.Criar(useCase.Object, request, CancellationToken.None);

        // Assert
        var created = result as CreatedAtActionResult;
        created.Should().NotBeNull();
        created!.ActionName.Should().Be(nameof(MetasController.ObterPorNome));

        var response = created.Value as CreateMetaResponse;
        response.Should().NotBeNull();
        response!.Message.Should().Contain("Meta criada");

        useCase.Verify(x => x.ExecuteAsync("Carro", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ObterPorNome_Deve_Retornar_Ok_Com_Meta()
    {
        // Arrange
        var meta = new Meta("Carro");

        var useCase = new Mock<IObterMetaPorNomeUseCase>();
        useCase.Setup(x => x.ExecuteAsync("Carro", It.IsAny<CancellationToken>()))
               .ReturnsAsync(meta);

        var controller = new MetasController();

        // Act
        var result = await controller.ObterPorNome(
            useCase.Object,
            "Carro",
            CancellationToken.None);

        // Assert
        var ok = result.Result as OkObjectResult;
        ok.Should().NotBeNull();

        var response = ok!.Value as MetaResponse;
        response.Should().NotBeNull();
        response!.Nome.Should().Be("Carro");

        useCase.Verify(x => x.ExecuteAsync("Carro", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Listar_Deve_Retornar_Ok_Com_Metas()
    {
        // Arrange
        var metas = new List<Meta>
        {
            new Meta("Carro"),
            new Meta("Casa")
        };

        var useCase = new Mock<IListarMetasUseCase>();
        useCase.Setup(x => x.ExecuteAsync(It.IsAny<CancellationToken>()))
               .ReturnsAsync(metas);

        var controller = new MetasController();

        // Act
        var result = await controller.Listar(useCase.Object, CancellationToken.None);

        // Assert
        var ok = result as OkObjectResult;
        ok.Should().NotBeNull();

        var list = ok!.Value as IEnumerable<object>;
        list.Should().NotBeNull();
        list!.Count().Should().Be(2);

        useCase.Verify(x => x.ExecuteAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Depositar_Deve_Retornar_Ok()
    {
        // Arrange
        var useCase = new Mock<IDepositarUseCase>();
        useCase.Setup(x =>
                x.ExecuteAsync("Carro", 100m, "pix", It.IsAny<CancellationToken>()))
               .Returns(Task.CompletedTask);

        var controller = new MetasController();
        var request = new TransacaoRequest();

        request.Valor = 100m;
        request.Descricao = "pix";
        

        // Act
        var result = await controller.Depositar(
            useCase.Object,
            "Carro",
            request,
            CancellationToken.None);

        // Assert
        var ok = result as OkObjectResult;
        ok.Should().NotBeNull();

        var response = ok!.Value as TransacaoResponse;
        response.Should().NotBeNull();
        response!.Message.Should().Contain("Depósito");

        useCase.Verify(x =>
            x.ExecuteAsync("Carro", 100m, "pix", It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Sacar_Deve_Retornar_Ok()
    {
        // Arrange
        var useCase = new Mock<ISacarUseCase>();
        useCase.Setup(x =>
                x.ExecuteAsync("Carro", 50m, "saque", It.IsAny<CancellationToken>()))
               .Returns(Task.CompletedTask);

        var controller = new MetasController();
        var request = new TransacaoRequest();

        request.Valor = 50m;
        request.Descricao = "saque";

        // Act
        var result = await controller.Sacar(
            useCase.Object,
            "Carro",
            request,
            CancellationToken.None);

        // Assert
        var ok = result as OkObjectResult;
        ok.Should().NotBeNull();

        var response = ok!.Value as TransacaoResponse;
        response.Should().NotBeNull();
        response!.Message.Should().Contain("Saque");

        useCase.Verify(x =>
            x.ExecuteAsync("Carro", 50m, "saque", It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Extrato_Deve_Retornar_Extrato_Da_Meta_Quando_Nome_Informado()
    {
        // Arrange
        var extratoUseCase = new Mock<IGerarExtratoUseCase>();
        extratoUseCase.Setup(x =>
                x.ExecuteAsync("Carro", It.IsAny<CancellationToken>()))
            .ReturnsAsync("EXTRATO CARRO");

        var extratoGeralUseCase = new Mock<IGerarExtratoGeralUseCase>();

        var controller = new MetasController();

        // Act
        var result = await controller.Extrato(
            extratoUseCase.Object,
            extratoGeralUseCase.Object,
            "Carro",
            CancellationToken.None);

        // Assert
        var ok = result as OkObjectResult;
        ok.Should().NotBeNull();
        ok!.Value.Should().Be("EXTRATO CARRO");

        extratoUseCase.Verify(x =>
            x.ExecuteAsync("Carro", It.IsAny<CancellationToken>()),
            Times.Once);

        extratoGeralUseCase.Verify(x =>
            x.ExecuteAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Extrato_Deve_Retornar_Extrato_Geral_Quando_Nome_Nao_Informado()
    {
        // Arrange
        var extratoUseCase = new Mock<IGerarExtratoUseCase>();
        var extratoGeralUseCase = new Mock<IGerarExtratoGeralUseCase>();

        extratoGeralUseCase.Setup(x =>
                x.ExecuteAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync("EXTRATO GERAL");

        var controller = new MetasController();

        // Act
        var result = await controller.Extrato(
            extratoUseCase.Object,
            extratoGeralUseCase.Object,
            null,
            CancellationToken.None);

        // Assert
        var ok = result as OkObjectResult;
        ok.Should().NotBeNull();
        ok!.Value.Should().Be("EXTRATO GERAL");

        extratoUseCase.Verify(x =>
            x.ExecuteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);

        extratoGeralUseCase.Verify(x =>
            x.ExecuteAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
