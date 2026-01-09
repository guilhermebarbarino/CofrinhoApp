using Cofrinho.Application.Services.UseCases;
using Cofrinho.Domain.Entities;
using Cofrinho.Domain.Interfaces;
using Cofrinho.Domain.Utils;
using FluentAssertions;
using Moq;

public class ObterMetaPorNomeUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_Deve_Retornar_Meta_Quando_Existir()
    {
        // Arrange
        var meta = new Meta("Carro");

        var repo = new Mock<IMetaRepository>();
        repo.Setup(r => r.GetByName("Carro")).Returns(meta);

        var useCase = new ObterMetaPorNomeUseCase(repo.Object);

        // Act
        var resultado = await useCase.ExecuteAsync("Carro", CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Nome.Should().Be("Carro");

        repo.Verify(r => r.GetByName("Carro"), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_Deve_Lancar_ArgumentException_Se_Nome_Vazio()
    {
        // Arrange
        var repo = new Mock<IMetaRepository>();
        var useCase = new ObterMetaPorNomeUseCase(repo.Object);

        // Act
        Func<Task> act = () => useCase.ExecuteAsync("   ", CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<ArgumentException>()
            .WithMessage($"*{ErrorMessages.NomeMetaObrigatorio}*");

        repo.Verify(r => r.GetByName(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_Deve_Lancar_KeyNotFoundException_Se_Meta_Nao_Existir()
    {
        // Arrange
        var repo = new Mock<IMetaRepository>();
        repo.Setup(r => r.GetByName("Inexistente")).Returns((Meta?)null);

        var useCase = new ObterMetaPorNomeUseCase(repo.Object);

        // Act
        Func<Task> act = () => useCase.ExecuteAsync("Inexistente", CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<KeyNotFoundException>()
            .WithMessage($"*{ErrorMessages.MetaNaoEncontrada}*");

        repo.Verify(r => r.GetByName("Inexistente"), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_Deve_Chamar_GetByName_Com_Nome_Trimado()
    {
        // Arrange
        var meta = new Meta("Casa");

        var repo = new Mock<IMetaRepository>();
        repo.Setup(r => r.GetByName("Casa")).Returns(meta);

        var useCase = new ObterMetaPorNomeUseCase(repo.Object);

        // Act
        await useCase.ExecuteAsync("   Casa   ", CancellationToken.None);

        // Assert
        repo.Verify(r => r.GetByName("Casa"), Times.Once);
    }
}
