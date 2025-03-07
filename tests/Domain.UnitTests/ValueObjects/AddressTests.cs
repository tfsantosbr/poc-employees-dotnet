using Domain.ValueObjects;
using Xunit;

namespace Domain.UnitTests.ValueObjects
{
    public class AddressTests
    {
        [Fact]
        public void Should_CreateSuccessfully_When_AllFieldsAreValid()
        {
            // Arrange
            string street = "Main Street";
            string number = "123";
            string complement = "Apt 4B";
            string neighborhood = "Downtown";
            string city = "New York";
            string state = "NY";
            string zipCode = "10001";
            string country = "USA";

            // Act
            var result = Address.Create(
                street, number, complement, neighborhood,
                city, state, zipCode, country);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(street, result.Value.Street);
            Assert.Equal(number, result.Value.Number);
            Assert.Equal(complement, result.Value.Complement);
            Assert.Equal(neighborhood, result.Value.Neighborhood);
            Assert.Equal(city, result.Value.City);
            Assert.Equal(state, result.Value.State);
            Assert.Equal(zipCode, result.Value.ZipCode);
            Assert.Equal(country, result.Value.Country);
            Assert.False(result.Value.IsMain);
        }

        [Fact]
        public void Should_UseDefaultCountry_When_CountryIsNotProvided()
        {
            // Arrange
            string street = "Main Street";
            string number = "123";
            string complement = "Apt 4B";
            string neighborhood = "Downtown";
            string city = "São Paulo";
            string state = "SP";
            string zipCode = "01001-000";

            // Act
            var result = Address.Create(
                street, number, complement, neighborhood,
                city, state, zipCode);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Brasil", result.Value.Country);
        }

        [Theory]
        [InlineData("", "123", "Downtown", "New York", "NY", "10001", "Address.StreetEmpty")]
        [InlineData(null, "123", "Downtown", "New York", "NY", "10001", "Address.StreetEmpty")]
        [InlineData("  ", "123", "Downtown", "New York", "NY", "10001", "Address.StreetEmpty")]
        public void Should_ReturnFailure_When_StreetIsEmpty(
            string street, string number, string neighborhood,
            string city, string state, string zipCode, string expectedErrorCode)
        {
            // Act
            var result = Address.Create(
                street, number, "Complement", neighborhood,
                city, state, zipCode);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Contains(result.Errors, e => e.Code == expectedErrorCode);
        }

        [Theory]
        [InlineData("Main Street", "", "Downtown", "New York", "NY", "10001", "Address.NumberEmpty")]
        [InlineData("Main Street", null, "Downtown", "New York", "NY", "10001", "Address.NumberEmpty")]
        [InlineData("Main Street", "  ", "Downtown", "New York", "NY", "10001", "Address.NumberEmpty")]
        public void Should_ReturnFailure_When_NumberIsEmpty(
            string street, string number, string neighborhood,
            string city, string state, string zipCode, string expectedErrorCode)
        {
            // Act
            var result = Address.Create(
                street, number, "Complement", neighborhood,
                city, state, zipCode);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Contains(result.Errors, e => e.Code == expectedErrorCode);
        }

        [Theory]
        [InlineData("Main Street", "123", "", "New York", "NY", "10001", "Address.NeighborhoodEmpty")]
        [InlineData("Main Street", "123", null, "New York", "NY", "10001", "Address.NeighborhoodEmpty")]
        [InlineData("Main Street", "123", "  ", "New York", "NY", "10001", "Address.NeighborhoodEmpty")]
        public void Should_ReturnFailure_When_NeighborhoodIsEmpty(
            string street, string number, string neighborhood,
            string city, string state, string zipCode, string expectedErrorCode)
        {
            // Act
            var result = Address.Create(
                street, number, "Complement", neighborhood,
                city, state, zipCode);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Contains(result.Errors, e => e.Code == expectedErrorCode);
        }

        [Theory]
        [InlineData("Main Street", "123", "Downtown", "", "NY", "10001", "Address.CityEmpty")]
        [InlineData("Main Street", "123", "Downtown", null, "NY", "10001", "Address.CityEmpty")]
        [InlineData("Main Street", "123", "Downtown", "  ", "NY", "10001", "Address.CityEmpty")]
        public void Should_ReturnFailure_When_CityIsEmpty(
            string street, string number, string neighborhood,
            string city, string state, string zipCode, string expectedErrorCode)
        {
            // Act
            var result = Address.Create(
                street, number, "Complement", neighborhood,
                city, state, zipCode);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Contains(result.Errors, e => e.Code == expectedErrorCode);
        }

        [Theory]
        [InlineData("Main Street", "123", "Downtown", "New York", "", "10001", "Address.StateEmpty")]
        [InlineData("Main Street", "123", "Downtown", "New York", null, "10001", "Address.StateEmpty")]
        [InlineData("Main Street", "123", "Downtown", "New York", "  ", "10001", "Address.StateEmpty")]
        public void Should_ReturnFailure_When_StateIsEmpty(
            string street, string number, string neighborhood,
            string city, string state, string zipCode, string expectedErrorCode)
        {
            // Act
            var result = Address.Create(
                street, number, "Complement", neighborhood,
                city, state, zipCode);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Contains(result.Errors, e => e.Code == expectedErrorCode);
        }

        [Theory]
        [InlineData("Main Street", "123", "Downtown", "New York", "NY", "", "Address.ZipCodeEmpty")]
        [InlineData("Main Street", "123", "Downtown", "New York", "NY", null, "Address.ZipCodeEmpty")]
        [InlineData("Main Street", "123", "Downtown", "New York", "NY", "  ", "Address.ZipCodeEmpty")]
        public void Should_ReturnFailure_When_ZipCodeIsEmpty(
            string street, string number, string neighborhood,
            string city, string state, string zipCode, string expectedErrorCode)
        {
            // Act
            var result = Address.Create(
                street, number, "Complement", neighborhood,
                city, state, zipCode);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Contains(result.Errors, e => e.Code == expectedErrorCode);
        }

        [Fact]
        public void Should_AllowNullComplement_When_CreatingAddress()
        {
            // Arrange
            string street = "Main Street";
            string number = "123";
            string complement = null;
            string neighborhood = "Downtown";
            string city = "New York";
            string state = "NY";
            string zipCode = "10001";

            // Act
            var result = Address.Create(
                street, number, complement, neighborhood,
                city, state, zipCode);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Null(result.Value.Complement);
        }

        [Fact]
        public void Should_SetIsMain_When_CallingSetMainMethod()
        {
            // Arrange
            var addressResult = Address.Create(
                "Main Street", "123", "Apt 4B", "Downtown",
                "New York", "NY", "10001");
            var address = addressResult.Value;

            // Act
            address.SetMain(true);

            // Assert
            Assert.True(address.IsMain);

            // Act again
            address.SetMain(false);

            // Assert again
            Assert.False(address.IsMain);
        }

        [Fact]
        public void Should_BeEqual_When_AllPropertiesAreEqual()
        {
            // Arrange
            var address1 = Address.Create(
                "Main Street", "123", "Apt 4B", "Downtown",
                "New York", "NY", "10001").Value;

            var address2 = Address.Create(
                "Main Street", "123", "Apt 4B", "Downtown",
                "New York", "NY", "10001").Value;

            // Act & Assert
            Assert.Equal(address1, address2);
        }

        [Fact]
        public void Should_NotBeEqual_When_PropertiesAreDifferent()
        {
            // Arrange
            var address1 = Address.Create(
                "Main Street", "123", "Apt 4B", "Downtown",
                "New York", "NY", "10001").Value;

            var address2 = Address.Create(
                "Second Street", "456", "Apt 4B", "Downtown",
                "New York", "NY", "10001").Value;

            // Act & Assert
            Assert.NotEqual(address1, address2);
        }

        [Fact]
        public void Should_NotBeEqual_When_IsMainPropertyIsDifferent()
        {
            // Arrange
            var address1 = Address.Create(
                "Main Street", "123", "Apt 4B", "Downtown",
                "New York", "NY", "10001").Value;

            var address2 = Address.Create(
                "Main Street", "123", "Apt 4B", "Downtown",
                "New York", "NY", "10001").Value;
            
            // Make one address main
            address2.SetMain(true);

            // Act & Assert
            Assert.NotEqual(address1, address2);
        }
    }
}