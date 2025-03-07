using System;
using Application.Common;

namespace Application.Employees.Commands
{
    public record AddEmployeeAddressCommand(
        Guid EmployeeId,
        string Street,
        string Number,
        string Complement,
        string Neighborhood,
        string City,
        string State,
        string ZipCode,
        string Country,
        bool IsMain
    ) : ICommand
    {
        public string Country { get; init; } = Country ?? "Brasil";
    }
}
