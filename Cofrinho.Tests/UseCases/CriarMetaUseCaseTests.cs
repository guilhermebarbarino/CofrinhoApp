using Cofrinho.Application.UseCases;
using Cofrinho.Domain.Entities;
using Cofrinho.Domain.Interfaces;
using Cofrinho.Domain.Utils;
using Cofrinho.Infrastructure.UnitOfWork;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

public class CriarMetaUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_Deve_Criar_Meta_E_Salvar()
    {
        // Arrange
        var repo = new Mock<IMetaRepository>();
        repo.Setup(r => r.Exists("carro")).Returns(false);

        Meta? metaCriada = null;
        repo.Setup(r => r.Add(It.IsAny<Meta>()))
            .Callback<Meta>(m => metaCriada = m);

        var uow = new Mock<IUnitOfWork>();
        uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
           .ReturnsAsync(1); // <- ajuste para Task<int>, igual no caso anterior

        var logger = new Mock<ILogger<CriarMetaUseCase>>();

        var useCase = new CriarMetaUseCase(repo.Object, uow.Object, logger.Object);

        // Act
        await useCase.ExecuteAsync("carro", CancellationToken.None);

        // Assert
        metaCriada.Should().NotBeNull();
        metaCriada!.Nome.Should().Be("carro");

        repo.Verify(r => r.Exists("carro"), Times.Once);
        repo.Verify(r => r.Add(It.IsAny<Meta>()), Times.Once);
        uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_Deve_Lancar_ArgumentException_Se_Nome_Vazio()
    {
        // Arrange
        var repo = new Mock<IMetaRepository>();
        var uow = new Mock<IUnitOfWork>();
        var logger = new Mock<ILogger<CriarMetaUseCase>>();

        var useCase = new CriarMetaUseCase(repo.Object, uow.Object, logger.Object);

        // Act
        Func<Task> act = () => useCase.ExecuteAsync("   ", CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<ArgumentException>()
            .WithMessage($"*{ErrorMessages.NomeMetaObrigatorio}*");

        repo.Verify(r => r.Exists(It.IsAny<string>()), Times.Never);
        repo.Verify(r => r.Add(It.IsAny<Meta>()), Times.Never);
        uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_Deve_Lancar_InvalidOperationException_Se_Meta_Ja_Existe()
    {
        // Arrange
        var repo = new Mock<IMetaRepository>();
        repo.Setup(r => r.Exists("carro")).Returns(true);

        var uow = new Mock<IUnitOfWork>();
        var logger = new Mock<ILogger<CriarMetaUseCase>>();

        var useCase = new CriarMetaUseCase(repo.Object, uow.Object, logger.Object);

        // Act
        Func<Task> act = () => useCase.ExecuteAsync("carro", CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("*Já existe uma meta com esse nome.*");

        repo.Verify(r => r.Exists("carro"), Times.Once);
        repo.Verify(r => r.Add(It.IsAny<Meta>()), Times.Never);
        uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_Deve_Chamar_Exists_Com_Nome_Trimado()
    {
        // Arrange
        var repo = new Mock<IMetaRepository>();
        repo.Setup(r => r.Exists("carro")).Returns(false);

        var uow = new Mock<IUnitOfWork>();
        uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
           .ReturnsAsync(1);

        var logger = new Mock<ILogger<CriarMetaUseCase>>();

        var useCase = new CriarMetaUseCase(repo.Object, uow.Object, logger.Object);

        // Act
        await useCase.ExecuteAsync("  carro  ", CancellationToken.None);

        // Assert
        repo.Verify(r => r.Exists("carro"), Times.Once);
    }
}
