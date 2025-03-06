using System;
using System.Collections.Generic;

namespace Application.Employees.Models.Responses
{
    public class EmployeeResponse
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public DateTime BirthDate { get; set; }
        public string Document { get; set; }
        public string DocumentType { get; set; }
        public string Position { get; set; }
        public decimal Salary { get; set; }
        public string Currency { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsActive { get; set; }
        public List<AddressResponse> Addresses { get; set; } = new List<AddressResponse>();
    }
}
