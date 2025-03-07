using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Employees.Queries;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using Domain.ValueObjects;
using NSubstitute;
using Xunit;

namespace Application.UnitTests.Employees.Queries
{
    public class GetEmployeeByIdQueryHandlerTests
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly GetEmployeeByIdQueryHandler _handler;
        private readonly Guid _validEmployeeId = Guid.NewGuid();

        public GetEmployeeByIdQueryHandlerTests()
        {
            _employeeRepository = Substitute.For<IEmployeeRepository>();
            _handler = new GetEmployeeByIdQueryHandler(_employeeRepository);
        }

        [Fact]
        public async Task Should_ReturnEmployee_When_EmployeeExists()
        {
            // Arrange
            var employee = CreateValidEmployee();
            
            _employeeRepository
                .GetByIdAsync(_validEmployeeId, Arg.Any<CancellationToken>())
                .Returns(employee);

            var query = new GetEmployeeByIdQuery(_validEmployeeId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(_validEmployeeId, result.Value.Id);
            Assert.Equal("John", result.Value.FirstName);
            Assert.Equal("Doe", result.Value.LastName);
            Assert.Equal("John Doe", result.Value.FullName);
            Assert.Equal("john.doe@example.com", result.Value.Email);
            Assert.Equal(new DateTime(1990, 1, 1), result.Value.BirthDate);
            Assert.Equal("12345678901", result.Value.Document);
            Assert.Equal("CPF", result.Value.DocumentType);
            Assert.Equal("Developer", result.Value.Position);
            Assert.Equal(5000m, result.Value.Salary);
            Assert.Equal("BRL", result.Value.Currency);
            Assert.True(result.Value.IsActive);
            Assert.Empty(result.Value.Addresses);
        }

        [Fact]
        public async Task Should_ReturnNotFound_When_EmployeeDoesNotExist()
        {
            // Arrange
            _employeeRepository
                .GetByIdAsync(_validEmployeeId, Arg.Any<CancellationToken>())
                .Returns((Employee)null);

            var query = new GetEmployeeByIdQuery(_validEmployeeId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Contains(result.Errors, e => e.Code == "Employee.NotFound");
        }

        private Employee CreateValidEmployee()
        {
            var name = PersonName.Create("John", "Doe").Value;
            var email = Email.Create("john.doe@example.com").Value;
            var birthDate = new DateTime(1990, 1, 1);
            var document = Document.Create("12345678901").Value;
            var position = "Developer";
            var salary = Money.Create(5000m, "BRL").Value;

            var employee = Employee.Create(
                name,
                email,
                birthDate,
                document,
                position,
                salary);

            // Set the ID to a known value for testing
            var idField = typeof(Employee).GetField("_id", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            idField.SetValue(employee, _validEmployeeId);

            return employee;
        }
    }
}