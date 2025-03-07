using Domain.ValueObjects;
using Xunit;

namespace Domain.UnitTests.ValueObjects
{
    public class PersonNameTests
    {
        [Fact]
        public void Should_CreateSuccessfully_When_BothNamesAreValid()
        {
            // Arrange
            string firstName = "John";
            string lastName = "Doe";

            // Act
            var result = PersonName.Create(firstName, lastName);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(firstName, result.Value.FirstName);
            Assert.Equal(lastName, result.Value.LastName);
        }

        [Theory]
        [InlineData("", "Doe")]
        [InlineData(null, "Doe")]
        [InlineData("   ", "Doe")]
        public void Should_ReturnFailure_When_FirstNameIsEmpty(string firstName, string lastName)
        {
            // Act
            var result = PersonName.Create(firstName, lastName);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Contains(result.Errors, e => e.Code == "PersonName.FirstNameEmpty");
        }

        [Theory]
        [InlineData("John", "")]
        [InlineData("John", null)]
        [InlineData("John", "   ")]
        public void Should_ReturnFailure_When_LastNameIsEmpty(string firstName, string lastName)
        {
            // Act
            var result = PersonName.Create(firstName, lastName);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Contains(result.Errors, e => e.Code == "PersonName.LastNameEmpty");
        }

        [Fact]
        public void Should_ConcatenateNames_When_AccessingFullName()
        {
            // Arrange
            string firstName = "John";
            string lastName = "Doe";
            var personName = PersonName.Create(firstName, lastName).Value;

            // Act
            string fullName = personName.FullName;

            // Assert
            Assert.Equal($"{firstName} {lastName}", fullName);
        }

        [Fact]
        public void Should_BeEqual_When_BothNamesAreEqual()
        {
            // Arrange
            var name1 = PersonName.Create("John", "Doe").Value;
            var name2 = PersonName.Create("John", "Doe").Value;

            // Act & Assert
            Assert.Equal(name1, name2);
            Assert.True(name1 == name2);
        }

        [Fact]
        public void Should_NotBeEqual_When_NamesAreDifferent()
        {
            // Arrange
            var name1 = PersonName.Create("John", "Doe").Value;
            var name2 = PersonName.Create("Jane", "Doe").Value;

            // Act & Assert
            Assert.NotEqual(name1, name2);
            Assert.True(name1 != name2);
        }
    }
}