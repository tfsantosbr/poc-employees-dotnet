using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common;
using Domain.Common;
using Domain.Common.Errors;
using Domain.Repositories;
using Domain.ValueObjects;
using FluentValidation;

namespace Application.Employees.Commands
{
    public class UpdateEmployeeCommandHandler(
        IEmployeeRepository employeeRepository,
        IUnitOfWork unitOfWork,
        IValidator<UpdateEmployeeCommand> validator) : ICommandHandler<UpdateEmployeeCommand, Result>
    {
        private readonly IEmployeeRepository _employeeRepository = employeeRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IValidator<UpdateEmployeeCommand> _validator = validator;

        public async Task<Result> Handle(UpdateEmployeeCommand command, CancellationToken cancellationToken = default)
        {
            // Validar comando
            var validationResult = await _validator.ValidateAsync(command, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => new Error("Validation", e.ErrorMessage));
                return Result.Failure(errors);
            }

            // Verificar se o funcionário existe
            var employee = await _employeeRepository.GetByIdAsync(command.Id, cancellationToken);
            if (employee == null)
                return Result.Failure("NOT_FOUND", "Funcionário não encontrado");

            // Verificar se o email já existe (excluindo o funcionário atual)
            var emailExists = await _employeeRepository.EmailExistsAsync(command.Email, command.Id, cancellationToken);
            if (emailExists)
                return Result.Failure("DUPLICATE_EMAIL", "O email informado já está em uso por outro funcionário");

            // Criar os value objects
            var nameResult = PersonName.Create(command.FirstName, command.LastName);
            if (nameResult.IsFailure)
                return Result.Failure(nameResult.Errors);

            var emailResult = Email.Create(command.Email);
            if (emailResult.IsFailure)
                return Result.Failure(emailResult.Errors);

            var salaryResult = Money.Create(command.Salary, command.Currency);
            if (salaryResult.IsFailure)
                return Result.Failure(salaryResult.Errors);

            // Atualizar a entidade
            employee.Update(
                nameResult.Value,
                emailResult.Value,
                command.BirthDate,
                command.Position,
                salaryResult.Value);

            // Persistir a entidade
            await _employeeRepository.UpdateAsync(employee, cancellationToken);

            // Salvar as alterações
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
