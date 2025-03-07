using System;
using Application.Common;
using Application.Employees.Models.Responses;
using Domain.Common;

namespace Application.Employees.Commands
{
    public record CreateEmployeeCommand(
        string FirstName,
        string LastName,
        string Email,
        DateTime BirthDate,
        string Document,
        string Position,
        decimal Salary,
        string Currency
    ) : ICommand<Result<EmployeeResponse>>
    {
        public string Currency { get; init; } = Currency ?? "BRL";
    }
}
