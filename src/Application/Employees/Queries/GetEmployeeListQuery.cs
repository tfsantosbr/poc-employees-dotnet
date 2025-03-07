using Application.Common;
using Application.Employees.Models.Responses;
using Domain.Common;

namespace Application.Employees.Queries
{
    public record GetEmployeeListQuery(int Page = 1, int PageSize = 10) : IQuery<Result<EmployeeListResponse>>;
}
