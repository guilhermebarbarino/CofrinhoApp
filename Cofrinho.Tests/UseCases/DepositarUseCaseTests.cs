using Cofrinho.Application.UseCases;
using Cofrinho.Domain.Entities;
using Cofrinho.Domain.Enums;
using Cofrinho.Domain.Interfaces;
using Cofrinho.Domain.Utils;
using Cofrinho.Infrastructure.UnitOfWork;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

public class DepositarUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_Deve_Depositar_E_Salvar()
    {
        // Arrange
        var meta = new Meta("carro");

        var repo = new Mock<IMetaRepository>();
        repo.Setup(r => r.GetByName("carro")).Returns(meta);

        var uow = new Mock<IUnitOfWork>();

        uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
           .ReturnsAsync(1);
        var logger = new Mock<ILogger<DepositarUseCase>>();

        var useCase = new DepositarUseCase(repo.Object, uow.Object, logger.Object);

        // Act
        await useCase.ExecuteAsync("carro", 100m, "pix", CancellationToken.None);

        // Assert
        meta.Saldo.Should().Be(100m);
        meta.Transacoes.Should().HaveCount(1);
        meta.Transacoes
                     .First()
                     .Tipo
                     .Should()
                     .Be(TipoTransacao.Deposito);


        uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        repo.Verify(x => x.GetByName("carro"), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_Deve_Lancar_ArgumentException_Se_Nome_Vazio()
    {
        // Arrange
        var repo = new Mock<IMetaRepository>();
        var uow = new Mock<IUnitOfWork>();
        var logger = new Mock<ILogger<DepositarUseCase>>();
        var useCase = new DepositarUseCase(repo.Object, uow.Object, logger.Object);

        // Act
        Func<Task> act = () => useCase.ExecuteAsync("", 10m, "pix", CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<ArgumentException>()
            .WithMessage($"*{ErrorMessages.NomeMetaObrigatorio}*");

        uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        repo.Verify(x => x.GetByName(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_Deve_Lancar_ArgumentException_Se_Valor_Menor_Ou_Igual_Zero()
    {
        // Arrange
        var repo = new Mock<IMetaRepository>();
        var uow = new Mock<IUnitOfWork>();
        var logger = new Mock<ILogger<DepositarUseCase>>();
        var useCase = new DepositarUseCase(repo.Object, uow.Object, logger.Object);

        // Act
        Func<Task> act = () => useCase.ExecuteAsync("carro", 0m, "pix", CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<ArgumentException>()
            .WithMessage($"*{ErrorMessages.TransacaoValorMenorQueZero}*");

        uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        repo.Verify(x => x.GetByName(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_Deve_Lancar_KeyNotFound_Se_Meta_Nao_Existe()
    {
        // Arrange
        var repo = new Mock<IMetaRepository>();
        repo.Setup(r => r.GetByName("inexistente")).Returns((Meta?)null);

        var uow = new Mock<IUnitOfWork>();
        var logger = new Mock<ILogger<DepositarUseCase>>();

        var useCase = new DepositarUseCase(repo.Object, uow.Object, logger.Object);

        // Act
        Func<Task> act = () => useCase.ExecuteAsync("inexistente", 10m, "pix", CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<KeyNotFoundException>()
            .WithMessage($"*{ErrorMessages.MetaNaoEncontrada}*");

        uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        repo.Verify(x => x.GetByName("inexistente"), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_Deve_Chamar_GetByName_Com_Nome_Trimado()
    {
        // Arrange
        var meta = new Meta("carro");

        var repo = new Mock<IMetaRepository>();
        repo.Setup(r => r.GetByName("carro")).Returns(meta);

        var uow = new Mock<IUnitOfWork>();

        uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
           .ReturnsAsync(1);

        var logger = new Mock<ILogger<DepositarUseCase>>();
        var useCase = new DepositarUseCase(repo.Object, uow.Object, logger.Object);

        // Act
        await useCase.ExecuteAsync("  carro  ", 10m, "pix", CancellationToken.None);

        // Assert
        repo.Verify(r => r.GetByName("carro"), Times.Once);
    }
}
