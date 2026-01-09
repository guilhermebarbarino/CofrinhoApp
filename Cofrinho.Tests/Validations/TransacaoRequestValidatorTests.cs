using Cofrinho.Api.Contracts;
using Cofrinho.Api.Validation;
using Cofrinho.Domain.Utils;
using FluentAssertions;
using FluentValidation.TestHelper;

public class TransacaoRequestValidatorTests
{
    private readonly TransacaoRequestValidator _validator = new();

    [Fact]
    public void Deve_Erro_Quando_Valor_For_Zero()
    {
        // Arrange
        var request = new TransacaoRequest( );
        request.Valor = 0m;
        request.Descricao = "pix";

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Valor)
              .WithErrorMessage(ErrorMessages.TransacaoValorMenorQueZero);
    }

    [Fact]
    public void Deve_Erro_Quando_Valor_For_Negativo()
    {
        // Arrange
        var request = new TransacaoRequest();
        request.Valor = -5m;
        request.Descricao = "pix";

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Valor)
              .WithErrorMessage(ErrorMessages.TransacaoValorMenorQueZero);
    }

    [Fact]
    public void Deve_Passar_Quando_Valor_For_Positivo()
    {
        // Arrange
        var request = new TransacaoRequest();

        request.Valor = 15.75m;
        request.Descricao = "pix";

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Valor);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Deve_Passar_Quando_Descricao_For_Null_Ou_Vazia_Ou_Espacos()
    {
        // Arrange
        var nullDesc = new TransacaoRequest();
        nullDesc.Valor = 10m;
        nullDesc.Descricao = null;

        var emptyDesc = new TransacaoRequest();
        emptyDesc.Valor = 10m;
        emptyDesc.Descricao = string.Empty;


        var whiteDesc = new TransacaoRequest();
        whiteDesc.Valor = 10m;
        whiteDesc.Descricao = "   ";

        // Act
        var nullResult = _validator.TestValidate(nullDesc);
        var emptyResult = _validator.TestValidate(emptyDesc);
        var whiteResult = _validator.TestValidate(whiteDesc);

        // Assert
        nullResult.IsValid.Should().BeTrue();
        emptyResult.IsValid.Should().BeTrue();
        whiteResult.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Deve_Erro_Quando_Descricao_Tiver_Mais_De_140_Caracteres()
    {
        // Arrange
        var descricao = new string('A', 141);
        var request = new TransacaoRequest();
        request.Valor = 10m;
        request.Descricao = descricao;

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Descricao)
              .WithErrorMessage("Descrição deve ter no máximo 140 caracteres.");
    }

    [Fact]
    public void Deve_Passar_Quando_Descricao_Tiver_Exatamente_140_Caracteres()
    {
        // Arrange
        var descricao = new string('A', 140);
        var request = new TransacaoRequest();
        request.Valor = 10m;
        request.Descricao = descricao;

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Descricao);
        result.IsValid.Should().BeTrue();
    }
}
