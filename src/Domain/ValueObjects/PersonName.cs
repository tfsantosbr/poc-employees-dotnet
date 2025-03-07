using Domain.Common;

namespace Domain.ValueObjects
{
    public class PersonName : ValueObject
    {
        private PersonName(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }

        public string FirstName { get; }
        public string LastName { get; }

        public string FullName => $"{FirstName} {LastName}";

        public static Result<PersonName> Create(string firstName, string lastName)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                return Result.Failure<PersonName>("PersonName.FirstNameEmpty", "O nome não pode ser vazio");

            if (string.IsNullOrWhiteSpace(lastName))
                return Result.Failure<PersonName>("PersonName.LastNameEmpty", "O sobrenome não pode ser vazio");

            return Result.Success(new PersonName(firstName, lastName));
        }

        protected override object[] GetEqualityComponents()
        {
            return new object[] { FirstName, LastName };
        }
    }
}