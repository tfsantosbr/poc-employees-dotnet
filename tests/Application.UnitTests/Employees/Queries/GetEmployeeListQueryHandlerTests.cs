using System;
using System.Collections.Generic;
using System.Linq;
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
    public class GetEmployeeListQueryHandlerTests
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly GetEmployeeListQueryHandler _handler;

        public GetEmployeeListQueryHandlerTests()
        {
            _employeeRepository = Substitute.For<IEmployeeRepository>();
            _handler = new GetEmployeeListQueryHandler(_employeeRepository);
        }

        [Fact]
        public async Task Should_ReturnEmployeeList_When_EmployeesExist()
        {
            // Arrange
            var employees = new List<Employee>
            {
                CreateEmployee("John", "Doe", "john.doe@example.com", "12345678901", 5000m),
                CreateEmployee("Jane", "Smith", "jane.smith@example.com", "12345678902", 6000m)
            };

            _employeeRepository
                .GetAllAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
                .Returns(employees);

            var query = new GetEmployeeListQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            var itemsList = result.Value.Employees.ToList();
            Assert.Equal(2, itemsList.Count);

            var employee1 = itemsList[0];
            Assert.Equal("John Doe", employee1.FullName);
            Assert.Equal("john.doe@example.com", employee1.Email);
            Assert.Equal("12345678901", employee1.Document);
            Assert.Equal("Developer", employee1.Position);
            Assert.Equal(5000m, employee1.Salary);
            Assert.True(employee1.IsActive);

            var employee2 = itemsList[1];
            Assert.Equal("Jane Smith", employee2.FullName);
            Assert.Equal("jane.smith@example.com", employee2.Email);
            Assert.Equal("12345678902", employee2.Document);
            Assert.Equal("Developer", employee2.Position);
            Assert.Equal(6000m, employee2.Salary);
            Assert.True(employee2.IsActive);
        }

        [Fact]
        public async Task Should_ReturnEmptyList_When_NoEmployeesExist()
        {
            // Arrange
            _employeeRepository
                .GetAllAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
                .Returns(new List<Employee>());

            var query = new GetEmployeeListQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Empty(result.Value.Employees);
        }

        private Employee CreateEmployee(string firstName, string lastName, string email, string document, decimal salary)
        {
            var name = PersonName.Create(firstName, lastName).Value;
            var emailObj = Email.Create(email).Value;
            var birthDate = new DateTime(1990, 1, 1);
            var documentObj = Document.Create(document).Value;
            var position = "Developer";
            var salaryObj = Money.Create(salary, "BRL").Value;

            return Employee.Create(
                name,
                emailObj,
                birthDate,
                documentObj,
                position,
                salaryObj);
        }
    }
}