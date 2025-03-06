using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common;
using Application.Employees.Commands;
using Application.Employees.Models.Responses;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using Domain.ValueObjects;
using FluentValidation;

namespace Application.Employees.Handlers
{
    public class CreateEmployeeCommandHandler : ICommandHandler<CreateEmployeeCommand, Result<EmployeeResponse>>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<CreateEmployeeCommand> _validator;

        public CreateEmployeeCommandHandler(
            IEmployeeRepository employeeRepository,
            IUnitOfWork unitOfWork,
            IValidator<CreateEmployeeCommand> validator)
        {
            _employeeRepository = employeeRepository;
            _unitOfWork = unitOfWork;
            _validator = validator;
        }

        public async Task<Result<EmployeeResponse>> Handle(CreateEmployeeCommand command, CancellationToken cancellationToken = default)
        {
            // Validar comando
            var validationResult = await _validator.ValidateAsync(command, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result.Failure<EmployeeResponse>(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            // Verificar se o email já existe
            var emailExistsResult = await _employeeRepository.EmailExistsAsync(command.Email, null, cancellationToken);
            if (emailExistsResult.IsFailure)
                return Result.Failure<EmployeeResponse>(emailExistsResult.Errors);

            if (emailExistsResult.Value)
                return Result.Failure<EmployeeResponse>("O email informado já está em uso");

            // Verificar se o documento já existe
            var documentExistsResult = await _employeeRepository.DocumentExistsAsync(command.Document, null, cancellationToken);
            if (documentExistsResult.IsFailure)
                return Result.Failure<EmployeeResponse>(documentExistsResult.Errors);

            if (documentExistsResult.Value)
                return Result.Failure<EmployeeResponse>("O documento informado já está em uso");

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
            var employeeResult = Employee.Create(
                nameResult.Value,
                emailResult.Value,
                command.BirthDate,
                documentResult.Value,
                command.Position,
                salaryResult.Value);

            if (employeeResult.IsFailure)
                return Result.Failure<EmployeeResponse>(employeeResult.Errors);

            // Persistir a entidade
            var employee = employeeResult.Value;
            var addResult = await _employeeRepository.AddAsync(employee, cancellationToken);
            if (addResult.IsFailure)
                return Result.Failure<EmployeeResponse>(addResult.Errors);

            // Salvar as alterações
            var saveResult = await _unitOfWork.SaveChangesAsync(cancellationToken);
            if (saveResult.IsFailure)
                return Result.Failure<EmployeeResponse>(saveResult.Errors);

            // Retornar o resultado
            return Result.Success(MapToResponse(employee));
        }

        private static EmployeeResponse MapToResponse(Employee employee)
        {
            return new EmployeeResponse
            {
                Id = employee.Id,
                FirstName = employee.Name.FirstName,
                LastName = employee.Name.LastName,
                FullName = employee.Name.FullName,
                Email = employee.Email.Value,
                BirthDate = employee.BirthDate,
                Document = employee.Document.Value,
                DocumentType = employee.Document.Type.ToString(),
                Position = employee.Position,
                Salary = employee.Salary.Amount,
                Currency = employee.Salary.Currency,
                CreatedAt = employee.CreatedAt,
                UpdatedAt = employee.UpdatedAt,
                IsActive = employee.IsActive,
                Addresses = employee.Addresses.Select(a => new AddressResponse
                {
                    Street = a.Street,
                    Number = a.Number,
                    Complement = a.Complement,
                    Neighborhood = a.Neighborhood,
                    City = a.City,
                    State = a.State,
                    ZipCode = a.ZipCode,
                    Country = a.Country,
                    IsMain = a.IsMain
                }).ToList()
            };
        }
    }
}
