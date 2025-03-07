using System;
using System.Collections.Generic;

namespace Application.Employees.Models.Responses
{
    public record EmployeeResponse(
        Guid Id,
        string FirstName,
        string LastName,
        string FullName,
        string Email,
        DateTime BirthDate,
        string Document,
        string DocumentType,
        string Position,
        decimal Salary,
        string Currency,
        DateTime CreatedAt,
        DateTime UpdatedAt,
        bool IsActive,
        List<AddressResponse> Addresses
    )
    {
        public List<AddressResponse> Addresses { get; init; } = Addresses ?? new List<AddressResponse>();
    }
}
