using System.Text.RegularExpressions;
using Domain.Common;

namespace Domain.ValueObjects
{
    public class Email : ValueObject
    {
        private Email(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public static Result<Email> Create(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return Result.Failure<Email>("Email.Empty", "O email não pode ser vazio");

            // Simple email validation
            var pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(email, pattern))
                return Result.Failure<Email>("Email.InvalidFormat", "Formato de email inválido");

            return Result.Success(new Email(email));
        }

        protected override object[] GetEqualityComponents()
        {
            return new object[] { Value.ToLowerInvariant() };
        }
        
        public override string ToString() => Value;
    }
}