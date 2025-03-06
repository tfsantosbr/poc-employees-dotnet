using System;
using Application.Common;
using Application.Employees.Models.Responses;
using Domain.Common;

namespace Application.Employees.Commands
{
    public class CreateEmployeeCommand : ICommand<Result<EmployeeResponse>>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime BirthDate { get; set; }
        public string Document { get; set; }
        public string Position { get; set; }
        public decimal Salary { get; set; }
        public string Currency { get; set; } = "BRL";
    }
}
