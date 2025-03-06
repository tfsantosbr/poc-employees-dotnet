using System;
using Application.Employees.Commands;
using FluentValidation;

namespace Application.Employees.Validators
{
    public class CreateEmployeeCommandValidator : AbstractValidator<CreateEmployeeCommand>
    {
        public CreateEmployeeCommandValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("O nome é obrigatório")
                .MinimumLength(2).WithMessage("O nome deve ter pelo menos 2 caracteres")
                .MaximumLength(50).WithMessage("O nome deve ter no máximo 50 caracteres");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("O sobrenome é obrigatório")
                .MinimumLength(2).WithMessage("O sobrenome deve ter pelo menos 2 caracteres")
                .MaximumLength(50).WithMessage("O sobrenome deve ter no máximo 50 caracteres");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("O email é obrigatório")
                .EmailAddress().WithMessage("O email é inválido")
                .MaximumLength(254).WithMessage("O email deve ter no máximo 254 caracteres");

            RuleFor(x => x.BirthDate)
                .NotEmpty().WithMessage("A data de nascimento é obrigatória")
                .Must(BeValidBirthDate).WithMessage("O funcionário deve ter pelo menos 18 anos")
                .Must(BeInThePast).WithMessage("A data de nascimento não pode ser no futuro");

            RuleFor(x => x.Document)
                .NotEmpty().WithMessage("O documento é obrigatório")
                .Must(BeValidDocument).WithMessage("Documento inválido. Deve ser CPF (11 dígitos) ou CNPJ (14 dígitos)");

            RuleFor(x => x.Position)
                .NotEmpty().WithMessage("O cargo é obrigatório")
                .MinimumLength(2).WithMessage("O cargo deve ter pelo menos 2 caracteres")
                .MaximumLength(100).WithMessage("O cargo deve ter no máximo 100 caracteres");

            RuleFor(x => x.Salary)
                .GreaterThan(0).WithMessage("O salário deve ser maior que zero");
        }

        private bool BeValidBirthDate(DateTime birthDate)
        {
            var minDate = new DateTime(1900, 1, 1);
            if (birthDate < minDate)
                return false;

            var age = DateTime.UtcNow.Year - birthDate.Year;
            if (DateTime.UtcNow.DayOfYear < birthDate.DayOfYear)
                age--;

            return age >= 18;
        }

        private bool BeInThePast(DateTime birthDate)
        {
            return birthDate < DateTime.UtcNow;
        }

        private bool BeValidDocument(string document)
        {
            if (string.IsNullOrWhiteSpace(document))
                return false;

            // Remove non-numeric characters
            var numericDocument = System.Text.RegularExpressions.Regex.Replace(document, @"[^\d]", "");

            return numericDocument.Length == 11 || numericDocument.Length == 14;
        }
    }
}
