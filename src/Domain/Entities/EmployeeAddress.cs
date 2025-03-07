using System;
using Domain.ValueObjects;

namespace Domain.Entities
{
    public class EmployeeAddress
    {
        private EmployeeAddress(
            Guid id,
            Guid employeeId,
            Address address)
        {
            Id = id;
            EmployeeId = employeeId;
            Address = address;
            CreatedAt = DateTime.UtcNow;
        }

        public Guid Id { get; private set; }
        public Guid EmployeeId { get; private set; }
        public Address Address { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public static EmployeeAddress Create(
            Guid employeeId,
            Address address)
        {
            return new EmployeeAddress(
                Guid.NewGuid(),
                employeeId,
                address);
        }

        public void SetAddressAsMain(bool isMain)
        {
            Address.SetMain(isMain);
        }
    }
}
