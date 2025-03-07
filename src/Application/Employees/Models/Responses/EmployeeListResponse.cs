using System;
using System.Collections.Generic;

namespace Application.Employees.Models.Responses
{
    public record EmployeeListResponse(
        IEnumerable<EmployeeListItemResponse> Employees,
        int TotalCount,
        int Page,
        int PageSize)
    {
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }
}
