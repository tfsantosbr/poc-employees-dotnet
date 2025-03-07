using System;
using Application.Common;

namespace Application.Employees.Commands
{
    public record UpdateEmployeeCommand(
        Guid Id,
        string FirstName,
        string LastName,
        string Email,
        DateTime BirthDate,
        string Position,
        decimal Salary,
        string Currency
    ) : ICommand
    {
        public string Currency { get; init; } = Currency ?? "BRL";
    }
}
