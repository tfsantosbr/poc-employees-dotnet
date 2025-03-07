using System;
using Application.Common;

namespace Application.Employees.Commands
{
    public record DeleteEmployeeCommand(Guid Id) : ICommand;
}
