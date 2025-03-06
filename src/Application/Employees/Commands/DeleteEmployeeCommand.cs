using System;
using Application.Common;
using Domain.Common;

namespace Application.Employees.Commands
{
    public class DeleteEmployeeCommand : ICommand
    {
        public Guid Id { get; set; }
    }
}
