using Cofrinho.Application.UseCases;
using Cofrinho.Domain.Entities;
using Cofrinho.Domain.Interfaces;
using FluentAssertions;
using Moq;

public class ListarMetasUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_Deve_Retornar_Lista_De_Metas()
    {
        // Arrange
        var metas = new List<Meta>
        {
            new Meta("Carro"),
            new Meta("Casa")
        };

        var repo = new Mock<IMetaRepository>();
        repo.Setup(r => r.GetAll()).Returns(metas);

        var useCase = new ListarMetasUseCase(repo.Object);

        // Act
        var resultado = await useCase.ExecuteAsync(CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().HaveCount(2);
        resultado.Should().Contain(m => m.Nome == "Carro");
        resultado.Should().Contain(m => m.Nome == "Casa");

        repo.Verify(r => r.GetAll(), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_Deve_Retornar_Colecao_Vazia_Se_Nao_Houver_Metas()
    {
        // Arrange
        var repo = new Mock<IMetaRepository>();
        repo.Setup(r => r.GetAll()).Returns(new List<Meta>());

        var useCase = new ListarMetasUseCase(repo.Object);

        // Act
        var resultado = await useCase.ExecuteAsync(CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().BeEmpty();

        repo.Verify(r => r.GetAll(), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_Deve_Retornar_IReadOnlyCollection()
    {
        // Arrange
        var metas = new List<Meta>
        {
            new Meta("Viagem")
        };

        var repo = new Mock<IMetaRepository>();
        repo.Setup(r => r.GetAll()).Returns(metas);

        var useCase = new ListarMetasUseCase(repo.Object);

        // Act
        var resultado = await useCase.ExecuteAsync(CancellationToken.None);

        // Assert
        resultado.Should().BeAssignableTo<IReadOnlyCollection<Meta>>();
    }
}
