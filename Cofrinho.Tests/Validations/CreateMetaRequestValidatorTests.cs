using Cofrinho.Api.Contracts;
using Cofrinho.Api.Validation;
using Cofrinho.Domain.Utils;
using FluentAssertions;
using FluentValidation.TestHelper;

public class CreateMetaRequestValidatorTests
{
    private readonly CreateMetaRequestValidator _validator = new();

    [Fact]
    public void Deve_Erro_Quando_Nome_Esta_Vazio()
    {
        // Arrange
        var request = new CreateMetaRequest();
        request.Nome = string.Empty;

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Nome)
              .WithErrorMessage(ErrorMessages.NomeMetaObrigatorio);
    }

    [Fact]
    public void Deve_Erro_Quando_Nome_Tem_Menos_De_2_Caracteres()
    {
        // Arrange
        var request = new CreateMetaRequest();
        request.Nome = "A";

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Nome)
              .WithErrorMessage("Nome da meta deve ter pelo menos 2 caracteres.");
    }

    [Fact]
    public void Deve_Erro_Quando_Nome_Tem_Mais_De_60_Caracteres()
    {
        // Arrange
        var nome = new string('A', 61);
        var request = new CreateMetaRequest();
        request.Nome = nome;

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Nome)
              .WithErrorMessage("Nome da meta deve ter no máximo 60 caracteres.");
    }

    [Fact]
    public void Deve_Passar_Quando_Nome_Eh_Valido()
    {
        // Arrange
        var request = new CreateMetaRequest();
        request.Nome = "Meta Valida";

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Nome);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Deve_Aceitar_Nome_Com_Exatamente_2_E_60_Caracteres()
    {
        // Arrange
        var min = new CreateMetaRequest();
        min.Nome = "AB";
        var max = new CreateMetaRequest();
        max.Nome = new string('A', 60);

        // Act
        var minResult = _validator.TestValidate(min);
        var maxResult = _validator.TestValidate(max);

        // Assert
        minResult.IsValid.Should().BeTrue();
        maxResult.IsValid.Should().BeTrue();
    }
}
