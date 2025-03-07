using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common;
using Application.Common.Interfaces;
using Application.Employees.Models.Responses;
using Domain.Common;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Employees.Queries
{
    public class GetEmployeeByIdQueryHandler(IApplicationDbContext dbContext) : IQueryHandler<GetEmployeeByIdQuery, Result<EmployeeResponse>>
    {
        private readonly IApplicationDbContext _dbContext = dbContext;

        public async Task<Result<EmployeeResponse>> Handle(GetEmployeeByIdQuery query, CancellationToken cancellationToken = default)
        {
            var employee = await _dbContext.Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == query.Id, cancellationToken);

            if (employee == null)
                return Result.Failure<EmployeeResponse>("NOT_FOUND", "Funcionário não encontrado");

            return Result.Success(MapToResponse(employee));
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
