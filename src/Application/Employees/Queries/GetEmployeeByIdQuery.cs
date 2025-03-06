using System;
using Application.Common;
using Application.Employees.Models.Responses;
using Domain.Common;

namespace Application.Employees.Queries
{
    public class GetEmployeeByIdQuery : IQuery<Result<EmployeeResponse>>
    {
        public Guid Id { get; set; }
    }
}
