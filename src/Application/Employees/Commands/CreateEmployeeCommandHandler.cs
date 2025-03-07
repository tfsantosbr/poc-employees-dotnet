using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common;
using Application.Employees.Models.Responses;
using Domain.Common;
using Domain.Common.Errors;
using Domain.Entities;
using Domain.Repositories;
using Domain.ValueObjects;
using FluentValidation;

namespace Application.Employees.Commands
{
    public class CreateEmployeeCommandHandler(
        IEmployeeRepository employeeRepository,
        IUnitOfWork unitOfWork,
        IValidator<CreateEmployeeCommand> validator) : ICommandHandler<CreateEmployeeCommand, Result<EmployeeResponse>>
    {
        private readonly IEmployeeRepository _employeeRepository = employeeRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IValidator<CreateEmployeeCommand> _validator = validator;

        public async Task<Result<EmployeeResponse>> Handle(CreateEmployeeCommand command, CancellationToken cancellationToken = default)
        {
            // Validar comando
            var validationResult = await _validator.ValidateAsync(command, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => new Error("Validation", e.ErrorMessage));
                return Result.Failure<EmployeeResponse>(errors);
            }

            // Verificar se o email já existe
            var emailExists = await _employeeRepository.EmailExistsAsync(command.Email, null, cancellationToken);
            if (emailExists)
                return Result.Failure<EmployeeResponse>("DUPLICATE_EMAIL", "O email informado já está em uso");

            // Verificar se o documento já existe
            var documentExists = await _employeeRepository.DocumentExistsAsync(command.Document, null, cancellationToken);
            if (documentExists)
                return Result.Failure<EmployeeResponse>("DUPLICATE_DOCUMENT", "O documento informado já está em uso");

            // Criar os value objects
            var nameResult = PersonName.Create(command.FirstName, command.LastName);
            if (nameResult.IsFailure)
                return Result.Failure<EmployeeResponse>(nameResult.Errors);

            var emailResult = Email.Create(command.Email);
            if (emailResult.IsFailure)
                return Result.Failure<EmployeeResponse>(emailResult.Errors);

            var documentResult = Document.Create(command.Document);
            if (documentResult.IsFailure)
                return Result.Failure<EmployeeResponse>(documentResult.Errors);

            var salaryResult = Money.Create(command.Salary, command.Currency);
            if (salaryResult.IsFailure)
                return Result.Failure<EmployeeResponse>(salaryResult.Errors);

            // Criar a entidade
            var employee = Employee.Create(
                nameResult.Value,
                emailResult.Value,
                command.BirthDate,
                documentResult.Value,
                command.Position,
                salaryResult.Value);

            // Persistir a entidade
            var addedEmployee = await _employeeRepository.AddAsync(employee, cancellationToken);

            // Salvar as alterações
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Retornar o resultado
            return Result.Success(MapToResponse(addedEmployee));
        }

        private static EmployeeResponse MapToResponse(Employee employee)
        {
            return new EmployeeResponse(
                Id: employee.Id,
                FirstName: employee.Name.FirstName,
                LastName: employee.Name.LastName,
                FullName: employee.Name.FullName,
                Email: employee.Email.Value,
                BirthDate: employee.BirthDate,
                Document: employee.Document.Value,
                DocumentType: employee.Document.Type.ToString(),
                Position: employee.Position,
                Salary: employee.Salary.Amount,
                Currency: employee.Salary.Currency,
                CreatedAt: employee.CreatedAt,
                UpdatedAt: employee.UpdatedAt,
                IsActive: employee.IsActive,
                Addresses: employee.Addresses.Select(a => new AddressResponse(
                    Street: a.Address.Street,
                    Number: a.Address.Number,
                    Complement: a.Address.Complement,
                    Neighborhood: a.Address.Neighborhood,
                    City: a.Address.City,
                    State: a.Address.State,
                    ZipCode: a.Address.ZipCode,
                    Country: a.Address.Country,
                    IsMain: a.Address.IsMain
                )).ToList()
            );
        }
    }
}
