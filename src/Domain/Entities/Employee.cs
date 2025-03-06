using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Common;
using Domain.ValueObjects;

namespace Domain.Entities
{
    public class Employee
    {
        private readonly List<Address> _addresses = new();
        
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
        public IReadOnlyCollection<Address> Addresses => _addresses.AsReadOnly();

        public static Result<Employee> Create(
            PersonName name,
            Email email,
            DateTime birthDate,
            Document document,
            string position,
            Money salary)
        {
            if (name == null)
                return Result.Failure<Employee>("O nome é obrigatório");
                
            if (email == null)
                return Result.Failure<Employee>("O email é obrigatório");
                
            if (document == null)
                return Result.Failure<Employee>("O documento é obrigatório");
                
            if (string.IsNullOrWhiteSpace(position))
                return Result.Failure<Employee>("O cargo é obrigatório");
                
            if (salary == null)
                return Result.Failure<Employee>("O salário é obrigatório");
                
            if (birthDate == default)
                return Result.Failure<Employee>("A data de nascimento é obrigatória");
                
            var minDate = new DateTime(1900, 1, 1);
            if (birthDate < minDate)
                return Result.Failure<Employee>("A data de nascimento não pode ser anterior a 01/01/1900");
                
            if (birthDate > DateTime.UtcNow)
                return Result.Failure<Employee>("A data de nascimento não pode ser no futuro");
                
            var age = DateTime.UtcNow.Year - birthDate.Year;
            if (DateTime.UtcNow.DayOfYear < birthDate.DayOfYear)
                age--;
                
            if (age < 18)
                return Result.Failure<Employee>("O funcionário deve ter pelo menos 18 anos");

            return Result.Success(new Employee(
                Guid.NewGuid(),
                name,
                email,
                birthDate,
                document,
                position,
                salary));
        }

        public Result Update(
            PersonName name,
            Email email,
            DateTime birthDate,
            string position,
            Money salary)
        {
            if (name == null)
                return Result.Failure("O nome é obrigatório");
                
            if (email == null)
                return Result.Failure("O email é obrigatório");
                
            if (string.IsNullOrWhiteSpace(position))
                return Result.Failure("O cargo é obrigatório");
                
            if (salary == null)
                return Result.Failure("O salário é obrigatório");
                
            if (birthDate == default)
                return Result.Failure("A data de nascimento é obrigatória");
                
            var minDate = new DateTime(1900, 1, 1);
            if (birthDate < minDate)
                return Result.Failure("A data de nascimento não pode ser anterior a 01/01/1900");
                
            if (birthDate > DateTime.UtcNow)
                return Result.Failure("A data de nascimento não pode ser no futuro");
                
            var age = DateTime.UtcNow.Year - birthDate.Year;
            if (DateTime.UtcNow.DayOfYear < birthDate.DayOfYear)
                age--;
                
            if (age < 18)
                return Result.Failure("O funcionário deve ter pelo menos 18 anos");
            
            Name = name;
            Email = email;
            BirthDate = birthDate;
            Position = position;
            Salary = salary;
            UpdatedAt = DateTime.UtcNow;
            
            return Result.Success();
        }
        
        public Result AddAddress(Address address)
        {
            if (address == null)
                return Result.Failure("O endereço não pode ser nulo");
            
            // If this is the first address or it's set as main, make sure it's marked as main
            if (!_addresses.Any() || address.IsMain)
            {
                // Set all existing addresses as non-main
                foreach (var existingAddress in _addresses.Where(a => a.IsMain))
                {
                    existingAddress.SetMain(false);
                }
            }
            
            _addresses.Add(address);
            UpdatedAt = DateTime.UtcNow;
            return Result.Success();
        }
        
        public Result RemoveAddress(Address addressToRemove)
        {
            if (addressToRemove == null)
                return Result.Failure("O endereço não pode ser nulo");
                
            var address = _addresses.FirstOrDefault(a => a.Equals(addressToRemove));
            if (address == null)
                return Result.Failure("Endereço não encontrado");
                
            _addresses.Remove(address);
            
            // If the removed address was the main one and we have other addresses, set the first one as main
            if (address.IsMain && _addresses.Any())
            {
                _addresses.First().SetMain(true);
            }
            
            UpdatedAt = DateTime.UtcNow;
            return Result.Success();
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