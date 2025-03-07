using System;
using System.Collections.Generic;
using System.Linq;
using Domain.ValueObjects;

namespace Domain.Entities
{
    public class Employee
    {
        private readonly List<EmployeeAddress> addresses = new();

        // Private constructor for EF Core
        private Employee() { }

        private Employee(
            Guid id,
            PersonName name,
            Email email,
            DateTime birthDate,
            Document document,
            string position,
            Money salary)
        {
            Id = id;
            Name = name;
            Email = email;
            BirthDate = birthDate;
            Document = document;
            Position = position;
            Salary = salary;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            IsActive = true;
        }

        public Guid Id { get; private set; }
        public PersonName Name { get; private set; }
        public Email Email { get; private set; }
        public DateTime BirthDate { get; private set; }
        public Document Document { get; private set; }
        public string Position { get; private set; }
        public Money Salary { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public bool IsActive { get; private set; }
        public IReadOnlyCollection<EmployeeAddress> Addresses => addresses.AsReadOnly();

        public static Employee Create(
            PersonName name,
            Email email,
            DateTime birthDate,
            Document document,
            string position,
            Money salary)
        {
            return new Employee(
                Guid.NewGuid(),
                name,
                email,
                birthDate,
                document,
                position,
                salary);
        }

        public void Update(
            PersonName name,
            Email email,
            DateTime birthDate,
            string position,
            Money salary)
        {
            Name = name;
            Email = email;
            BirthDate = birthDate;
            Position = position;
            Salary = salary;
            UpdatedAt = DateTime.UtcNow;
        }

        public void AddAddress(Address address)
        {
            var employeeAddress = EmployeeAddress.Create(Id, address);

            // If this is the first address or it's set as main, make sure it's marked as main
            if (!addresses.Any() || address.IsMain)
            {
                // Set all existing addresses as non-main
                foreach (var existingAddress in addresses.Where(a => a.Address.IsMain))
                {
                    existingAddress.SetAddressAsMain(false);
                }
            }

            addresses.Add(employeeAddress);
            UpdatedAt = DateTime.UtcNow;
        }

        public bool RemoveAddress(Guid addressId)
        {
            var address = addresses.FirstOrDefault(a => a.Id == addressId);
            if (address == null)
                return false;

            addresses.Remove(address);

            // If the removed address was the main one and we have other addresses, set the first one as main
            if (address.Address.IsMain && addresses.Any())
            {
                addresses.First().SetAddressAsMain(true);
            }

            UpdatedAt = DateTime.UtcNow;
            return true;
        }

        public void Activate()
        {
            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
