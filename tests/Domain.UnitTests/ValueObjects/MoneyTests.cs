using Domain.ValueObjects;
using Xunit;

namespace Domain.UnitTests.ValueObjects
{
    public class MoneyTests
    {
        [Fact]
        public void Should_CreateSuccessfully_When_AmountIsValid()
        {
            // Arrange
            decimal amount = 100.50m;
            string currency = "USD";

            // Act
            var result = Money.Create(amount, currency);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(amount, result.Value.Amount);
            Assert.Equal(currency, result.Value.Currency);
        }

        [Fact]
        public void Should_UseDefaultCurrency_When_CurrencyIsNotProvided()
        {
            // Arrange
            decimal amount = 100.50m;

            // Act
            var result = Money.Create(amount);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("BRL", result.Value.Currency);
        }

        [Fact]
        public void Should_ReturnFailure_When_AmountIsNegative()
        {
            // Arrange
            decimal negativeAmount = -10.00m;

            // Act
            var result = Money.Create(negativeAmount);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Contains(result.Errors, e => e.Code == "Money.NegativeAmount");
        }

        [Fact]
        public void Should_AddMoney_When_CurrenciesAreTheSame()
        {
            // Arrange
            var money1 = Money.Create(100.00m, "BRL").Value;
            var money2 = Money.Create(50.00m, "BRL").Value;
            
            // Act
            var result = money1.Add(money2);
            
            // Assert
            Assert.Equal(150.00m, result.Amount);
            Assert.Equal("BRL", result.Currency);
        }

        [Fact]
        public void Should_ThrowException_When_AddingDifferentCurrencies()
        {
            // Arrange
            var money1 = Money.Create(100.00m, "BRL").Value;
            var money2 = Money.Create(50.00m, "USD").Value;
            
            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => money1.Add(money2));
            Assert.Contains("Não é possível adicionar valores em moedas diferentes", exception.Message);
        }

        [Fact]
        public void Should_SubtractMoney_When_ResultIsPositive()
        {
            // Arrange
            var money1 = Money.Create(100.00m, "BRL").Value;
            var money2 = Money.Create(50.00m, "BRL").Value;
            
            // Act
            var result = money1.Subtract(money2);
            
            // Assert
            Assert.Equal(50.00m, result.Amount);
            Assert.Equal("BRL", result.Currency);
        }

        [Fact]
        public void Should_ThrowException_When_SubtractingDifferentCurrencies()
        {
            // Arrange
            var money1 = Money.Create(100.00m, "BRL").Value;
            var money2 = Money.Create(50.00m, "USD").Value;
            
            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => money1.Subtract(money2));
            Assert.Contains("Não é possível subtrair valores em moedas diferentes", exception.Message);
        }

        [Fact]
        public void Should_ThrowException_When_SubtractionResultIsNegative()
        {
            // Arrange
            var money1 = Money.Create(50.00m, "BRL").Value;
            var money2 = Money.Create(100.00m, "BRL").Value;
            
            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => money1.Subtract(money2));
            Assert.Contains("O resultado da subtração não pode ser negativo", exception.Message);
        }

        [Fact]
        public void Should_BeEqual_When_AmountAndCurrencyAreEqual()
        {
            // Arrange
            var money1 = Money.Create(100.00m, "BRL").Value;
            var money2 = Money.Create(100.00m, "BRL").Value;

            // Act & Assert
            Assert.Equal(money1, money2);
            Assert.True(money1 == money2);
        }

        [Fact]
        public void Should_NotBeEqual_When_AmountsAreDifferent()
        {
            // Arrange
            var money1 = Money.Create(100.00m, "BRL").Value;
            var money2 = Money.Create(200.00m, "BRL").Value;

            // Act & Assert
            Assert.NotEqual(money1, money2);
            Assert.True(money1 != money2);
        }

        [Fact]
        public void Should_NotBeEqual_When_CurrenciesAreDifferent()
        {
            // Arrange
            var money1 = Money.Create(100.00m, "BRL").Value;
            var money2 = Money.Create(100.00m, "USD").Value;

            // Act & Assert
            Assert.NotEqual(money1, money2);
            Assert.True(money1 != money2);
        }

        [Fact]
        public void Should_ReturnFormattedString_When_ConvertedToString()
        {
            // Arrange
            var money = Money.Create(100.50m, "USD").Value;

            // Act
            string result = money.ToString();

            // Assert
            Assert.Equal("100.50 USD", result);
        }
    }
}