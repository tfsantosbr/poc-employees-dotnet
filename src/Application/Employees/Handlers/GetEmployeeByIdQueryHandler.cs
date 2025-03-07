using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Common;
using Application.Employees.Models.Responses;
using Application.Employees.Queries;
using Domain.Common;
using Domain.Repositories;

namespace Application.Employees.Handlers
{
    public class GetEmployeeByIdQueryHandler(IEmployeeRepository employeeRepository) : IQueryHandler<GetEmployeeByIdQuery, Result<EmployeeResponse>>
    {

        public async Task<Result<EmployeeResponse>> Handle(GetEmployeeByIdQuery query, CancellationToken cancellationToken = default)
        {
            var employee = await employeeRepository.GetByIdAsync(query.Id, cancellationToken);
            if (employee == null)
                return Result.Failure<EmployeeResponse>("NOT_FOUND", "Funcionário não encontrado");

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

            var response = new EmployeeResponse(
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

            return Result.Success(response);
        }
    }
}