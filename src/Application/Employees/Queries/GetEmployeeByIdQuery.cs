using System;
using Application.Common;
using Application.Employees.Models.Responses;
using Domain.Common;

namespace Application.Employees.Queries
{
    public record GetEmployeeByIdQuery(Guid Id) : IQuery<Result<EmployeeResponse>>;
}
