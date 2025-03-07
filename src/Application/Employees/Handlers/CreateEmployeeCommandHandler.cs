using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common;
using Application.Employees.Commands;
using Application.Employees.Models.Responses;
using Domain.Common;
using Domain.Common.Errors;
using Domain.Entities;
using Domain.Repositories;
using Domain.ValueObjects;
using FluentValidation;

namespace Application.Employees.Handlers
{
    public class CreateEmployeeCommandHandler(
        IEmployeeRepository employeeRepository,
        IUnitOfWork unitOfWork,
        IValidator<CreateEmployeeCommand> validator) : ICommandHandler<CreateEmployeeCommand, Result<EmployeeResponse>>
    {

        public async Task<Result<EmployeeResponse>> Handle(CreateEmployeeCommand command, CancellationToken cancellationToken = default)
        {
            // Validar comando
            var validationResult = await validator.ValidateAsync(command, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result.Failure<EmployeeResponse>(validationResult.Errors.Select(e => new Error(e.ErrorCode, e.ErrorMessage)));
            }

            // Verificar se o email já existe
            var emailExists = await employeeRepository.EmailExistsAsync(command.Email, null, cancellationToken);
            if (emailExists)
                return Result.Failure<EmployeeResponse>("EmailInUse", "O email informado já está em uso");

            // Verificar se o documento já existe
            var documentExists = await employeeRepository.DocumentExistsAsync(command.Document, null, cancellationToken);
            if (documentExists)
                return Result.Failure<EmployeeResponse>("DocumentInUse", "O documento informado já está em uso");

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
            await employeeRepository.AddAsync(employee, cancellationToken);

            // Salvar as alterações
            await unitOfWork.SaveChangesAsync(cancellationToken);

            // Retornar o resultado
            return Result.Success(MapToResponse(employee));
        }

        private static EmployeeResponse MapToResponse(Employee employee)
        {
            var addresses = new List<AddressResponse>();

            foreach (var addr in employee.Addresses)
            {
                addresses.Add(new AddressResponse(
                    Street: addr.Address.Street,
                    Number: addr.Address.Number,
                    Complement: addr.Address.Complement,
                    Neighborhood: addr.Address.Neighborhood,
                    City: addr.Address.City,
                    State: addr.Address.State,
                    ZipCode: addr.Address.ZipCode,
                    Country: addr.Address.Country,
                    IsMain: addr.Address.IsMain
                ));
            }

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
                Addresses: addresses
            );
        }
    }
}