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
            if (!validationResult.IsValid)
            {
                return Result.Failure(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            // Verificar se o funcionário existe
            var employeeResult = await _employeeRepository.GetByIdAsync(command.EmployeeId, cancellationToken);
            if (employeeResult.IsFailure)
                return Result.Failure(employeeResult.Errors);

            var employee = employeeResult.Value;

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
            var addAddressResult = employee.AddAddress(addressResult.Value);
            if (addAddressResult.IsFailure)
                return Result.Failure(addAddressResult.Errors);

            // Persistir a entidade
            var updateResult = await _employeeRepository.UpdateAsync(employee, cancellationToken);
            if (updateResult.IsFailure)
                return Result.Failure(updateResult.Errors);

            // Salvar as alterações
            var saveResult = await _unitOfWork.SaveChangesAsync(cancellationToken);
            if (saveResult.IsFailure)
                return Result.Failure(saveResult.Errors);

            return Result.Success();
        }
    }
}
