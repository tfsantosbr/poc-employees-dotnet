using System;
using System.Collections.Generic;

namespace Application.Employees.Models.Responses
{
    public class EmployeeListResponse
    {
        public IEnumerable<EmployeeListItemResponse> Employees { get; set; }
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }

    public class EmployeeListItemResponse
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Document { get; set; }
        public string Position { get; set; }
        public decimal Salary { get; set; }
        public bool IsActive { get; set; }
    }
}
