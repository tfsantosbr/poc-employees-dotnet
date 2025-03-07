using System;

namespace Application.Employees.Models.Responses
{
    public record EmployeeListItemResponse(
        Guid Id,
        string FullName,
        string Email,
        string Document,
        string Position,
        decimal Salary,
        bool IsActive
    );
}
