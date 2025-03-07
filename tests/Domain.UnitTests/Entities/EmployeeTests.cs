using System;
using Domain.Entities;
using Domain.ValueObjects;
using Xunit;

namespace Domain.UnitTests.Entities
{
    public class EmployeeTests
    {
        private readonly PersonName _validName;
        private readonly Email _validEmail;
        private readonly DateTime _validBirthDate;
        private readonly Document _validDocument;
        private readonly string _validPosition;
        private readonly Money _validSalary;

        public EmployeeTests()
        {
            _validName = PersonName.Create("John", "Doe").Value;
            _validEmail = Email.Create("john.doe@example.com").Value;
            _validBirthDate = new DateTime(1990, 1, 1);
            _validDocument = Document.Create("12345678901").Value;
            _validPosition = "Developer";
            _validSalary = Money.Create(5000m, "BRL").Value;
        }

        [Fact]
        public void Should_CreateEmployee_When_AllParametersAreValid()
        {
            // Act
            var employee = Employee.Create(
                _validName,
                _validEmail,
                _validBirthDate,
                _validDocument,
                _validPosition,
                _validSalary);

            // Assert
            Assert.NotNull(employee);
            Assert.Equal(_validName, employee.Name);
            Assert.Equal(_validEmail, employee.Email);
            Assert.Equal(_validBirthDate, employee.BirthDate);
            Assert.Equal(_validDocument, employee.Document);
            Assert.Equal(_validPosition, employee.Position);
            Assert.Equal(_validSalary, employee.Salary);
            Assert.True(employee.IsActive);
            Assert.NotEqual(Guid.Empty, employee.Id);
            Assert.NotEqual(default, employee.CreatedAt);
            Assert.Equal(employee.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"), employee.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
            Assert.Empty(employee.Addresses);
        }

        [Fact]
        public void Should_UpdateInformation_When_UpdateMethodIsCalled()
        {
            // Arrange
            var employee = Employee.Create(
                _validName,
                _validEmail,
                _validBirthDate,
                _validDocument,
                _validPosition,
                _validSalary);

            var newName = PersonName.Create("Jane", "Doe").Value;
            var newEmail = Email.Create("jane.doe@example.com").Value;
            var newBirthDate = new DateTime(1995, 5, 5);
            var newPosition = "Senior Developer";
            var newSalary = Money.Create(8000m, "BRL").Value;

            // Act
            employee.Update(newName, newEmail, newBirthDate, newPosition, newSalary);

            // Assert
            Assert.Equal(newName, employee.Name);
            Assert.Equal(newEmail, employee.Email);
            Assert.Equal(newBirthDate, employee.BirthDate);
            Assert.Equal(newPosition, employee.Position);
            Assert.Equal(newSalary, employee.Salary);
            Assert.True(employee.UpdatedAt > employee.CreatedAt);
        }

        [Fact]
        public void Should_DeactivateEmployee_When_DeactivateMethodIsCalled()
        {
            // Arrange
            var employee = Employee.Create(
                _validName,
                _validEmail,
                _validBirthDate,
                _validDocument,
                _validPosition,
                _validSalary);

            // Act
            employee.Deactivate();

            // Assert
            Assert.False(employee.IsActive);
            Assert.True(employee.UpdatedAt > employee.CreatedAt);
        }

        [Fact]
        public void Should_ActivateEmployee_When_ActivateMethodIsCalled()
        {
            // Arrange
            var employee = Employee.Create(
                _validName,
                _validEmail,
                _validBirthDate,
                _validDocument,
                _validPosition,
                _validSalary);

            employee.Deactivate();
            Assert.False(employee.IsActive);

            // Act
            employee.Activate();

            // Assert
            Assert.True(employee.IsActive);
            Assert.True(employee.UpdatedAt > employee.CreatedAt);
        }

        [Fact]
        public void Should_AddAddress_When_AddAddressMethodIsCalled()
        {
            // Arrange
            var employee = Employee.Create(
                _validName,
                _validEmail,
                _validBirthDate,
                _validDocument,
                _validPosition,
                _validSalary);

            var address = Address.Create(
                "Main Street", "123", "Apt 4B", "Downtown",
                "New York", "NY", "10001").Value;

            // Act
            employee.AddAddress(address);

            // Assert
            Assert.Single(employee.Addresses);
            Assert.Equal(address, employee.Addresses.First().Address);
            Assert.True(address.IsMain); // First address should be set as main
            Assert.True(employee.UpdatedAt > employee.CreatedAt);
        }

        [Fact]
        public void Should_SetFirstAddressAsMain_When_AddingFirstAddress()
        {
            // Arrange
            var employee = Employee.Create(
                _validName,
                _validEmail,
                _validBirthDate,
                _validDocument,
                _validPosition,
                _validSalary);

            var address = Address.Create(
                "Main Street", "123", "Apt 4B", "Downtown",
                "New York", "NY", "10001").Value;

            // Act
            employee.AddAddress(address);

            // Assert
            Assert.True(address.IsMain);
        }

        [Fact]
        public void Should_NotChangeMainAddress_When_AddingNonMainAddress()
        {
            // Arrange
            var employee = Employee.Create(
                _validName,
                _validEmail,
                _validBirthDate,
                _validDocument,
                _validPosition,
                _validSalary);

            var address1 = Address.Create(
                "Main Street", "123", "Apt 4B", "Downtown",
                "New York", "NY", "10001").Value;

            var address2 = Address.Create(
                "Second Street", "456", null, "Uptown",
                "New York", "NY", "10002").Value;

            // Act
            employee.AddAddress(address1);
            employee.AddAddress(address2);

            // Assert
            Assert.Equal(2, employee.Addresses.Count);
            Assert.True(address1.IsMain);
            Assert.False(address2.IsMain);
        }

        [Fact]
        public void Should_ChangeMainAddress_When_AddingMainAddress()
        {
            // Arrange
            var employee = Employee.Create(
                _validName,
                _validEmail,
                _validBirthDate,
                _validDocument,
                _validPosition,
                _validSalary);

            var address1 = Address.Create(
                "Main Street", "123", "Apt 4B", "Downtown",
                "New York", "NY", "10001").Value;

            var address2 = Address.Create(
                "Second Street", "456", null, "Uptown",
                "New York", "NY", "10002").Value;
            address2.SetMain(true);

            // Act
            employee.AddAddress(address1);
            employee.AddAddress(address2);

            // Assert
            Assert.Equal(2, employee.Addresses.Count);
            Assert.False(address1.IsMain);
            Assert.True(address2.IsMain);
        }

        [Fact]
        public void Should_RemoveAddress_When_RemoveAddressMethodIsCalled()
        {
            // Arrange
            var employee = Employee.Create(
                _validName,
                _validEmail,
                _validBirthDate,
                _validDocument,
                _validPosition,
                _validSalary);

            var address1 = Address.Create(
                "Main Street", "123", "Apt 4B", "Downtown",
                "New York", "NY", "10001").Value;

            var address2 = Address.Create(
                "Second Street", "456", null, "Uptown",
                "New York", "NY", "10002").Value;

            employee.AddAddress(address1);
            employee.AddAddress(address2);
            Assert.Equal(2, employee.Addresses.Count);

            // Act
            var addressId = employee.Addresses.First(a => a.Address == address1).Id;
            employee.RemoveAddress(addressId);

            // Assert
            Assert.Single(employee.Addresses);
            Assert.Equal(address2, employee.Addresses.First().Address);
            Assert.True(address2.IsMain); // Last remaining address should be set as main
            Assert.True(employee.UpdatedAt > employee.CreatedAt);
        }

        [Fact]
        public void Should_SetNewMainAddress_When_RemovingMainAddress()
        {
            // Arrange
            var employee = Employee.Create(
                _validName,
                _validEmail,
                _validBirthDate,
                _validDocument,
                _validPosition,
                _validSalary);

            var address1 = Address.Create(
                "Main Street", "123", "Apt 4B", "Downtown",
                "New York", "NY", "10001").Value;

            var address2 = Address.Create(
                "Second Street", "456", null, "Uptown",
                "New York", "NY", "10002").Value;

            var address3 = Address.Create(
                "Third Street", "789", null, "Midtown",
                "New York", "NY", "10003").Value;

            employee.AddAddress(address1); // This will be main
            employee.AddAddress(address2);
            employee.AddAddress(address3);
            
            // Verify initial state
            Assert.True(address1.IsMain);
            Assert.False(address2.IsMain);
            Assert.False(address3.IsMain);

            // Act
            var addressId = employee.Addresses.First(a => a.Address == address1).Id;
            employee.RemoveAddress(addressId);

            // Assert
            Assert.Equal(2, employee.Addresses.Count);
            Assert.True(address2.IsMain); // First non-main address becomes main
            Assert.False(address3.IsMain);
        }

        [Fact]
        public void Should_NotThrowException_When_RemovingNonExistentAddress()
        {
            // Arrange
            var employee = Employee.Create(
                _validName,
                _validEmail,
                _validBirthDate,
                _validDocument,
                _validPosition,
                _validSalary);

            var address1 = Address.Create(
                "Main Street", "123", "Apt 4B", "Downtown",
                "New York", "NY", "10001").Value;

            var address2 = Address.Create(
                "Second Street", "456", null, "Uptown",
                "New York", "NY", "10002").Value;

            employee.AddAddress(address1);
            
            // Act & Assert
            var nonExistentAddressId = Guid.NewGuid();
            var exception = Record.Exception(() => employee.RemoveAddress(nonExistentAddressId));
            Assert.Null(exception);
            Assert.Single(employee.Addresses);
        }
    }
}