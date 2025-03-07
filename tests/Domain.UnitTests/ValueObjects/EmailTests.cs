using Domain.ValueObjects;
using Xunit;

namespace Domain.UnitTests.ValueObjects
{
    public class EmailTests
    {
        [Fact]
        public void Should_CreateSuccessfully_When_EmailIsValid()
        {
            // Arrange
            string validEmail = "test@example.com";

            // Act
            var result = Email.Create(validEmail);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(validEmail, result.Value.Value);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public void Should_ReturnFailure_When_EmailIsEmpty(string emptyEmail)
        {
            // Act
            var result = Email.Create(emptyEmail);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Contains(result.Errors, e => e.Code == "Email.Empty");
        }

        [Theory]
        [InlineData("invalid-email")]
        [InlineData("test@")]
        [InlineData("@example.com")]
        [InlineData("test@example")]
        public void Should_ReturnFailure_When_EmailFormatIsInvalid(string invalidEmail)
        {
            // Act
            var result = Email.Create(invalidEmail);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Contains(result.Errors, e => e.Code == "Email.InvalidFormat");
        }

        [Fact]
        public void Should_BeEqual_When_EmailValuesAreEqual()
        {
            // Arrange
            var email1 = Email.Create("test@example.com").Value;
            var email2 = Email.Create("test@example.com").Value;

            // Act & Assert
            Assert.Equal(email1, email2);
        }

        [Fact]
        public void Should_BeCaseInsensitive_When_ComparingEmails()
        {
            // Arrange
            var email1 = Email.Create("TEST@example.com").Value;
            var email2 = Email.Create("test@example.com").Value;

            // Act & Assert
            Assert.Equal(email1, email2);
        }

        [Fact]
        public void Should_ReturnValue_When_ConvertedToString()
        {
            // Arrange
            string emailValue = "test@example.com";
            var email = Email.Create(emailValue).Value;

            // Act
            string result = email.ToString();

            // Assert
            Assert.Equal(emailValue, result);
        }
    }
}