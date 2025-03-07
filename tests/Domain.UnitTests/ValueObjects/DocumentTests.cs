using Domain.ValueObjects;
using Xunit;

namespace Domain.UnitTests.ValueObjects
{
    public class DocumentTests
    {
        [Theory]
        [InlineData("12345678901")]            // CPF sem formatação
        [InlineData("123.456.789-01")]         // CPF formatado
        public void Should_CreateCPF_When_ValueHas11Digits(string documentValue)
        {
            // Act
            var result = Document.Create(documentValue);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("12345678901", result.Value.Value);
            Assert.Equal(Domain.ValueObjects.DocumentType.CPF, result.Value.Type);
        }

        [Theory]
        [InlineData("12345678901234")]         // CNPJ sem formatação
        [InlineData("12.345.678/9012-34")]     // CNPJ formatado
        public void Should_CreateCNPJ_When_ValueHas14Digits(string documentValue)
        {
            // Act
            var result = Document.Create(documentValue);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("12345678901234", result.Value.Value);
            Assert.Equal(Domain.ValueObjects.DocumentType.CNPJ, result.Value.Type);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public void Should_ReturnFailure_When_DocumentIsEmpty(string emptyDocument)
        {
            // Act
            var result = Document.Create(emptyDocument);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Contains(result.Errors, e => e.Code == "Document.Empty");
        }

        [Theory]
        [InlineData("1234567890")]         // 10 dígitos - inválido
        [InlineData("123456789012")]       // 12 dígitos - inválido
        [InlineData("1234567890123")]      // 13 dígitos - inválido
        [InlineData("123456789012345")]    // 15 dígitos - inválido
        public void Should_ReturnFailure_When_DocumentLengthIsInvalid(string invalidDocument)
        {
            // Act
            var result = Document.Create(invalidDocument);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Contains(result.Errors, e => e.Code == "Document.InvalidFormat");
        }

        [Fact]
        public void Should_RemoveNonDigits_When_CreatingDocument()
        {
            // Arrange
            string formattedDocument = "123.456.789-01";

            // Act
            var result = Document.Create(formattedDocument);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("12345678901", result.Value.Value);
        }

        [Fact]
        public void Should_BeEqual_When_DocumentValuesAreEqual()
        {
            // Arrange
            var document1 = Document.Create("123.456.789-01").Value;
            var document2 = Document.Create("12345678901").Value;

            // Act & Assert
            Assert.Equal(document1, document2);
        }

        [Fact]
        public void Should_NotBeEqual_When_DocumentValuesAreDifferent()
        {
            // Arrange
            var document1 = Document.Create("12345678901").Value;
            var document2 = Document.Create("12345678901234").Value;

            // Act & Assert
            Assert.NotEqual(document1, document2);
        }

        [Fact]
        public void Should_ReturnValue_When_ConvertedToString()
        {
            // Arrange
            var document = Document.Create("12345678901").Value;

            // Act
            string result = document.ToString();

            // Assert
            Assert.Equal("12345678901", result);
        }
    }
}