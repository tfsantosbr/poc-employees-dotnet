using System;
using Domain.Common;

namespace Domain.ValueObjects
{
    public class Address : ValueObject
    {
        private Address(
            string street,
            string number,
            string complement,
            string neighborhood,
            string city,
            string state,
            string zipCode,
            string country,
            bool isMain)
        {
            Street = street;
            Number = number;
            Complement = complement;
            Neighborhood = neighborhood;
            City = city;
            State = state;
            ZipCode = zipCode;
            Country = country;
            IsMain = isMain;
        }

        public string Street { get; }
        public string Number { get; }
        public string Complement { get; }
        public string Neighborhood { get; }
        public string City { get; }
        public string State { get; }
        public string ZipCode { get; }
        public string Country { get; }
        public bool IsMain { get; private set; }

        public static Result<Address> Create(
            string street,
            string number,
            string complement,
            string neighborhood,
            string city,
            string state,
            string zipCode,
            string country = "Brasil",
            bool isMain = false)
        {
            if (string.IsNullOrWhiteSpace(street))
                return Result.Failure<Address>("A rua não pode ser vazia");

            if (string.IsNullOrWhiteSpace(number))
                return Result.Failure<Address>("O número não pode ser vazio");

            if (string.IsNullOrWhiteSpace(neighborhood))
                return Result.Failure<Address>("O bairro não pode ser vazio");

            if (string.IsNullOrWhiteSpace(city))
                return Result.Failure<Address>("A cidade não pode ser vazia");

            if (string.IsNullOrWhiteSpace(state))
                return Result.Failure<Address>("O estado não pode ser vazio");

            if (string.IsNullOrWhiteSpace(zipCode))
                return Result.Failure<Address>("O CEP não pode ser vazio");

            return Result.Success(new Address(
                street,
                number,
                complement,
                neighborhood,
                city,
                state,
                zipCode,
                country,
                isMain));
        }

        public void SetMain(bool isMain)
        {
            IsMain = isMain;
        }

        protected override object[] GetEqualityComponents()
        {
            return new object[] 
            { 
                Street, 
                Number, 
                Complement, 
                Neighborhood, 
                City, 
                State, 
                ZipCode, 
                Country 
            };
        }
    }
}