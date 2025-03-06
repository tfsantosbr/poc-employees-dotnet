using Application.Employees.Commands;
using FluentValidation;

namespace Application.Employees.Validators
{
    public class AddEmployeeAddressCommandValidator : AbstractValidator<AddEmployeeAddressCommand>
    {
        public AddEmployeeAddressCommandValidator()
        {
            RuleFor(x => x.EmployeeId)
                .NotEmpty().WithMessage("O ID do funcionário é obrigatório");

            RuleFor(x => x.Street)
                .NotEmpty().WithMessage("A rua é obrigatória")
                .MaximumLength(100).WithMessage("A rua deve ter no máximo 100 caracteres");

            RuleFor(x => x.Number)
                .NotEmpty().WithMessage("O número é obrigatório")
                .MaximumLength(20).WithMessage("O número deve ter no máximo 20 caracteres");

            RuleFor(x => x.Complement)
                .MaximumLength(100).WithMessage("O complemento deve ter no máximo 100 caracteres");

            RuleFor(x => x.Neighborhood)
                .NotEmpty().WithMessage("O bairro é obrigatório")
                .MaximumLength(100).WithMessage("O bairro deve ter no máximo 100 caracteres");

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("A cidade é obrigatória")
                .MaximumLength(100).WithMessage("A cidade deve ter no máximo 100 caracteres");

            RuleFor(x => x.State)
                .NotEmpty().WithMessage("O estado é obrigatório")
                .MaximumLength(50).WithMessage("O estado deve ter no máximo 50 caracteres");

            RuleFor(x => x.ZipCode)
                .NotEmpty().WithMessage("O CEP é obrigatório")
                .MaximumLength(20).WithMessage("O CEP deve ter no máximo 20 caracteres");

            RuleFor(x => x.Country)
                .NotEmpty().WithMessage("O país é obrigatório")
                .MaximumLength(50).WithMessage("O país deve ter no máximo 50 caracteres");
        }
    }
}
