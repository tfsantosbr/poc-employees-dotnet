using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common;
using Domain.Common;
using Domain.Repositories;
using Domain.ValueObjects;
using FluentValidation;

namespace Application.Employees.Commands
{
    public class AddEmployeeAddressCommandHandler : ICommandHandler<AddEmployeeAddressCommand, Result>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<AddEmployeeAddressCommand> _validator;

        public AddEmployeeAddressCommandHandler(
            IEmployeeRepository employeeRepository,
            IUnitOfWork unitOfWork,
            IValidator<AddEmployeeAddressCommand> validator)
        {
            _employeeRepository = employeeRepository;
            _unitOfWork = unitOfWork;
            _validator = validator;
        }

        public async Task<Result> Handle(AddEmployeeAddressCommand command, CancellationToken cancellationToken = default)
        {
            // Validar comando
            var validationResult = await _validator.ValidateAsync(command, cancellationToken);
            if (\!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => new Error("Validation", e.ErrorMessage));
                return Result.Failure(errors);
            }

            // Verificar se o funcionário existe
            var employee = await _employeeRepository.GetByIdAsync(command.EmployeeId, cancellationToken);
            if (employee == null)
                return Result.Failure("NOT_FOUND", "Funcionário não encontrado");

            // Criar o endereço
            var addressResult = Address.Create(
                command.Street,
                command.Number,
                command.Complement,
                command.Neighborhood,
                command.City,
                command.State,
                command.ZipCode,
                command.Country,
                command.IsMain);

            if (addressResult.IsFailure)
                return Result.Failure(addressResult.Errors);

            // Adicionar o endereço ao funcionário
            employee.AddAddress(addressResult.Value);

            // Persistir a entidade
            await _employeeRepository.UpdateAsync(employee, cancellationToken);

            // Salvar as alterações
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
