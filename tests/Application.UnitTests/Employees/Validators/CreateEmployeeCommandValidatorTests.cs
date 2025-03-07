using System;
using Application.Employees.Commands;
using Application.Employees.Validators;
using ValidatorToTest = Application.Employees.Validators.CreateEmployeeCommandValidator;
using Xunit;

namespace Application.UnitTests.Employees.Validators
{
    public class CreateEmployeeCommandValidatorTests
    {
        private readonly ValidatorToTest _validator;

        public CreateEmployeeCommandValidatorTests()
        {
            _validator = new ValidatorToTest();
        }

        [Fact]
        public void Should_NotHaveErrors_When_CommandIsValid()
        {
            // Arrange
            var command = new CreateEmployeeCommand(
                FirstName: "John",
                LastName: "Doe",
                Email: "john.doe@example.com",
                BirthDate: new DateTime(1990, 1, 1),
                Document: "12345678901",
                Position: "Developer",
                Salary: 5000m,
                Currency: "BRL");

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public void Should_HaveError_When_FirstNameIsEmpty(string firstName)
        {
            // Arrange
            var command = new CreateEmployeeCommand(
                FirstName: firstName,
                LastName: "Doe",
                Email: "john.doe@example.com",
                BirthDate: new DateTime(1990, 1, 1),
                Document: "12345678901",
                Position: "Developer",
                Salary: 5000m,
                Currency: "BRL");

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "FirstName");
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public void Should_HaveError_When_LastNameIsEmpty(string lastName)
        {
            // Arrange
            var command = new CreateEmployeeCommand(
                FirstName: "John",
                LastName: lastName,
                Email: "john.doe@example.com",
                BirthDate: new DateTime(1990, 1, 1),
                Document: "12345678901",
                Position: "Developer",
                Salary: 5000m,
                Currency: "BRL");

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "LastName");
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        [InlineData("invalid-email")]
        [InlineData("john@")]
        [InlineData("@example.com")]
        public void Should_HaveError_When_EmailIsInvalid(string email)
        {
            // Arrange
            var command = new CreateEmployeeCommand(
                FirstName: "John",
                LastName: "Doe",
                Email: email,
                BirthDate: new DateTime(1990, 1, 1),
                Document: "12345678901",
                Position: "Developer",
                Salary: 5000m,
                Currency: "BRL");

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Email");
        }

        [Fact]
        public void Should_HaveError_When_BirthDateIsInFuture()
        {
            // Arrange
            var tomorrow = DateTime.Today.AddDays(1);
            var command = new CreateEmployeeCommand(
                FirstName: "John",
                LastName: "Doe",
                Email: "john.doe@example.com",
                BirthDate: tomorrow,
                Document: "12345678901",
                Position: "Developer",
                Salary: 5000m,
                Currency: "BRL");

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "BirthDate");
        }

        [Fact]
        public void Should_HaveError_When_EmployeeAgeLessThan18()
        {
            // Arrange
            var seventeenYearsAgo = DateTime.Today.AddYears(-17);
            var command = new CreateEmployeeCommand(
                FirstName: "John",
                LastName: "Doe",
                Email: "john.doe@example.com",
                BirthDate: seventeenYearsAgo,
                Document: "12345678901",
                Position: "Developer",
                Salary: 5000m,
                Currency: "BRL");

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "BirthDate");
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        [InlineData("123456")]
        [InlineData("123456789012345")]
        public void Should_HaveError_When_DocumentIsInvalid(string document)
        {
            // Arrange
            var command = new CreateEmployeeCommand(
                FirstName: "John",
                LastName: "Doe",
                Email: "john.doe@example.com",
                BirthDate: new DateTime(1990, 1, 1),
                Document: document,
                Position: "Developer",
                Salary: 5000m,
                Currency: "BRL");

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Document");
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public void Should_HaveError_When_PositionIsEmpty(string position)
        {
            // Arrange
            var command = new CreateEmployeeCommand(
                FirstName: "John",
                LastName: "Doe",
                Email: "john.doe@example.com",
                BirthDate: new DateTime(1990, 1, 1),
                Document: "12345678901",
                Position: position,
                Salary: 5000m,
                Currency: "BRL");

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Position");
        }

        [Fact]
        public void Should_HaveError_When_SalaryIsNegative()
        {
            // Arrange
            var command = new CreateEmployeeCommand(
                FirstName: "John",
                LastName: "Doe",
                Email: "john.doe@example.com",
                BirthDate: new DateTime(1990, 1, 1),
                Document: "12345678901",
                Position: "Developer",
                Salary: -1m,
                Currency: "BRL");

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Salary");
        }

        [Fact]
        public void Should_UseDefaultCurrency_When_CurrencyIsNull()
        {
            // Arrange
            var command = new CreateEmployeeCommand(
                FirstName: "John",
                LastName: "Doe",
                Email: "john.doe@example.com",
                BirthDate: new DateTime(1990, 1, 1),
                Document: "12345678901",
                Position: "Developer",
                Salary: 5000m,
                Currency: null);

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.True(result.IsValid);
            Assert.Equal("BRL", command.Currency);
        }
    }
}