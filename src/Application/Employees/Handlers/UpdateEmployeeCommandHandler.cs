using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common;
using Application.Employees.Commands;
using Domain.Common;
using Domain.Repositories;
using Domain.ValueObjects;
using FluentValidation;

namespace Application.Employees.Handlers
{
    public class UpdateEmployeeCommandHandler : ICommandHandler<UpdateEmployeeCommand, Result>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<UpdateEmployeeCommand> _validator;

        public UpdateEmployeeCommandHandler(
            IEmployeeRepository employeeRepository,
            IUnitOfWork unitOfWork,
            IValidator<UpdateEmployeeCommand> validator)
        {
            _employeeRepository = employeeRepository;
            _unitOfWork = unitOfWork;
            _validator = validator;
        }

        public async Task<Result> Handle(UpdateEmployeeCommand command, CancellationToken cancellationToken = default)
        {
            // Validar comando
            var validationResult = await _validator.ValidateAsync(command, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result.Failure(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            // Verificar se o funcionário existe
            var employeeResult = await _employeeRepository.GetByIdAsync(command.Id, cancellationToken);
            if (employeeResult.IsFailure)
                return Result.Failure(employeeResult.Errors);

            var employee = employeeResult.Value;

            // Verificar se o email já existe (excluindo o funcionário atual)
            var emailExistsResult = await _employeeRepository.EmailExistsAsync(command.Email, command.Id, cancellationToken);
            if (emailExistsResult.IsFailure)
                return Result.Failure(emailExistsResult.Errors);

            if (emailExistsResult.Value)
                return Result.Failure("O email informado já está em uso por outro funcionário");

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
            var updateResult = employee.Update(
                nameResult.Value,
                emailResult.Value,
                command.BirthDate,
                command.Position,
                salaryResult.Value);

            if (updateResult.IsFailure)
                return Result.Failure(updateResult.Errors);

            // Persistir a entidade
            var result = await _employeeRepository.UpdateAsync(employee, cancellationToken);
            if (result.IsFailure)
                return Result.Failure(result.Errors);

            // Salvar as alterações
            var saveResult = await _unitOfWork.SaveChangesAsync(cancellationToken);
            if (saveResult.IsFailure)
                return Result.Failure(saveResult.Errors);

            return Result.Success();
        }
    }
}