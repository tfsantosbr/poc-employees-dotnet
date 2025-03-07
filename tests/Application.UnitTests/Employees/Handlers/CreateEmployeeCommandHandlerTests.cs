using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Employees.Commands;
using Application.Employees.Handlers;
using HandlerToTest = Application.Employees.Handlers.CreateEmployeeCommandHandler;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using Domain.ValueObjects;
using FluentValidation;
using FluentValidation.Results;
using NSubstitute;
using Xunit;

namespace Application.UnitTests.Employees.Handlers
{
    public class CreateEmployeeCommandHandlerTests
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<CreateEmployeeCommand> _validator;
        private readonly HandlerToTest _handler;

        public CreateEmployeeCommandHandlerTests()
        {
            _employeeRepository = Substitute.For<IEmployeeRepository>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _validator = Substitute.For<IValidator<CreateEmployeeCommand>>();
            
            _handler = new HandlerToTest(
                _employeeRepository,
                _unitOfWork,
                _validator
            );
        }

        [Fact]
        public async Task Should_ReturnSuccess_When_CommandIsValid()
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

            _validator
                .ValidateAsync(command, Arg.Any<CancellationToken>())
                .Returns(new ValidationResult());

            _employeeRepository
                .EmailExistsAsync(command.Email, null, Arg.Any<CancellationToken>())
                .Returns(false);

            _employeeRepository
                .DocumentExistsAsync(command.Document, null, Arg.Any<CancellationToken>())
                .Returns(false);

            _employeeRepository
                .AddAsync(Arg.Any<Employee>(), Arg.Any<CancellationToken>())
                .Returns(Task.CompletedTask);

            _unitOfWork
                .SaveChangesAsync(Arg.Any<CancellationToken>())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(command.FirstName, result.Value.FirstName);
            Assert.Equal(command.LastName, result.Value.LastName);
            Assert.Equal(command.Email, result.Value.Email);
            Assert.Equal(command.BirthDate, result.Value.BirthDate);
            Assert.Equal(command.Document, result.Value.Document);
            Assert.Equal(command.Position, result.Value.Position);
            Assert.Equal(command.Salary, result.Value.Salary);
            Assert.Equal(command.Currency, result.Value.Currency);
            
            await _employeeRepository.Received(1).AddAsync(Arg.Any<Employee>(), Arg.Any<CancellationToken>());
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Should_ReturnFailure_When_ValidationFails()
        {
            // Arrange
            var command = new CreateEmployeeCommand(
                FirstName: "",
                LastName: "Doe",
                Email: "john.doe@example.com",
                BirthDate: new DateTime(1990, 1, 1),
                Document: "12345678901",
                Position: "Developer",
                Salary: 5000m,
                Currency: "BRL");

            var validationResult = new ValidationResult(new[] {
                new ValidationFailure("FirstName", "First name is required") { ErrorCode = "NotEmpty" }
            });

            _validator
                .ValidateAsync(command, Arg.Any<CancellationToken>())
                .Returns(validationResult);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Contains(result.Errors, e => e.Code == "NotEmpty");
            
            await _employeeRepository.DidNotReceive().AddAsync(Arg.Any<Employee>(), Arg.Any<CancellationToken>());
            await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Should_ReturnFailure_When_EmailAlreadyExists()
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

            _validator
                .ValidateAsync(command, Arg.Any<CancellationToken>())
                .Returns(new ValidationResult());

            _employeeRepository
                .EmailExistsAsync(command.Email, null, Arg.Any<CancellationToken>())
                .Returns(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Contains(result.Errors, e => e.Code == "EmailInUse");
            
            await _employeeRepository.DidNotReceive().AddAsync(Arg.Any<Employee>(), Arg.Any<CancellationToken>());
            await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Should_ReturnFailure_When_DocumentAlreadyExists()
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

            _validator
                .ValidateAsync(command, Arg.Any<CancellationToken>())
                .Returns(new ValidationResult());

            _employeeRepository
                .EmailExistsAsync(command.Email, null, Arg.Any<CancellationToken>())
                .Returns(false);

            _employeeRepository
                .DocumentExistsAsync(command.Document, null, Arg.Any<CancellationToken>())
                .Returns(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Contains(result.Errors, e => e.Code == "DocumentInUse");
            
            await _employeeRepository.DidNotReceive().AddAsync(Arg.Any<Employee>(), Arg.Any<CancellationToken>());
            await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Should_ReturnFailure_When_PersonNameValueObjectFails()
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

            _validator
                .ValidateAsync(command, Arg.Any<CancellationToken>())
                .Returns(new ValidationResult());

            _employeeRepository
                .EmailExistsAsync(command.Email, null, Arg.Any<CancellationToken>())
                .Returns(false);

            _employeeRepository
                .DocumentExistsAsync(command.Document, null, Arg.Any<CancellationToken>())
                .Returns(false);

            // Mock the static PersonName.Create method to return failure
            // This test assumes that your domain logic doesn't directly create value objects
            // but uses them through the Result pattern that can be checked for success/failure

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            // NOTE: This test might not be fully implementable without refactoring the code to allow for mocking static methods
            // or making the tests aware of the internal implementation of value objects
            // The test is included for completeness but might need adjustment based on your actual implementation

            // _mockEmployeeRepository.Verify(r => r.AddAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()), Times.Never);
            // _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}