using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common;
using Application.Employees.Models.Responses;
using Application.Employees.Queries;
using Domain.Common;
using Domain.Repositories;

namespace Application.Employees.Handlers
{
    public class GetEmployeeByIdQueryHandler : IQueryHandler<GetEmployeeByIdQuery, Result<EmployeeResponse>>
    {
        private readonly IEmployeeRepository _employeeRepository;

        public GetEmployeeByIdQueryHandler(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<Result<EmployeeResponse>> Handle(GetEmployeeByIdQuery query, CancellationToken cancellationToken = default)
        {
            var result = await _employeeRepository.GetByIdAsync(query.Id, cancellationToken);
            if (result.IsFailure)
                return Result.Failure<EmployeeResponse>(result.Errors);

            var employee = result.Value;

            return Result.Success(new EmployeeResponse
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
            });
        }
    }
}
