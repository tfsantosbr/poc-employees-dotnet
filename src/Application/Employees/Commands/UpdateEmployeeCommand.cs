using System;
using Application.Common;
using Domain.Common;

namespace Application.Employees.Commands
{
    public class UpdateEmployeeCommand : ICommand
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime BirthDate { get; set; }
        public string Position { get; set; }
        public decimal Salary { get; set; }
        public string Currency { get; set; } = "BRL";
    }
}
