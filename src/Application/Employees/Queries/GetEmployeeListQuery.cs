using Application.Common;
using Application.Employees.Models.Responses;
using Domain.Common;

namespace Application.Employees.Queries
{
    public class GetEmployeeListQuery : IQuery<Result<EmployeeListResponse>>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
