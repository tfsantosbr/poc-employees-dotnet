using System;
using Application.Employees.Commands;
using FluentValidation;

namespace Application.Employees.Validators
{
    public class UpdateEmployeeCommandValidator : AbstractValidator<UpdateEmployeeCommand>
    {
        public UpdateEmployeeCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("O ID do funcionário é obrigatório");

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
    }
}
