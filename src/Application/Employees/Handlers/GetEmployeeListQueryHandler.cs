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
    public class GetEmployeeListQueryHandler : IQueryHandler<GetEmployeeListQuery, Result<EmployeeListResponse>>
    {
        private readonly IEmployeeRepository _employeeRepository;

        public GetEmployeeListQueryHandler(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<Result<EmployeeListResponse>> Handle(GetEmployeeListQuery query, CancellationToken cancellationToken = default)
        {
            // Obter funcionários paginados
            var employeesResult = await _employeeRepository.GetAllAsync(query.Page, query.PageSize, cancellationToken);
            if (employeesResult.IsFailure)
                return Result.Failure<EmployeeListResponse>(employeesResult.Errors);

            // Obter contagem total
            var countResult = await _employeeRepository.GetTotalCountAsync(cancellationToken);
            if (countResult.IsFailure)
                return Result.Failure<EmployeeListResponse>(countResult.Errors);

            // Mapear resultados
            var response = new EmployeeListResponse
            {
                Employees = employeesResult.Value.Select(e => new EmployeeListItemResponse
                {
                    Id = e.Id,
                    FullName = e.Name.FullName,
                    Email = e.Email.Value,
                    Document = e.Document.Value,
                    Position = e.Position,
                    Salary = e.Salary.Amount,
                    IsActive = e.IsActive
                }),
                TotalCount = countResult.Value,
                Page = query.Page,
                PageSize = query.PageSize
            };

            return Result.Success(response);
        }
    }
}
