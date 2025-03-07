using System;
using Domain.Common;

namespace Domain.ValueObjects
{
    public class Money : ValueObject
    {
        private Money(decimal amount, string currency = "BRL")
        {
            Amount = amount;
            Currency = currency;
        }

        public decimal Amount { get; }
        public string Currency { get; }

        public static Result<Money> Create(decimal amount, string currency = "BRL")
        {
            if (amount < 0)
                return Result.Failure<Money>("Money.NegativeAmount", "O valor não pode ser negativo");

            return Result.Success(new Money(amount, currency));
        }
        
        public Money Add(Money money)
        {
            if (Currency != money.Currency)
                throw new InvalidOperationException("Não é possível adicionar valores em moedas diferentes");
                
            return new Money(Amount + money.Amount, Currency);
        }
        
        public Money Subtract(Money money)
        {
            if (Currency != money.Currency)
                throw new InvalidOperationException("Não é possível subtrair valores em moedas diferentes");
                
            var result = Amount - money.Amount;
            if (result < 0)
                throw new InvalidOperationException("O resultado da subtração não pode ser negativo");
                
            return new Money(result, Currency);
        }

        protected override object[] GetEqualityComponents()
        {
            return new object[] { Amount, Currency };
        }
        
        public override string ToString() => $"{Amount:F2} {Currency}";
    }
}