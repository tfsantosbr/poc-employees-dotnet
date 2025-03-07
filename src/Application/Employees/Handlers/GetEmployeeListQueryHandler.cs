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
    public class GetEmployeeListQueryHandler(IEmployeeRepository employeeRepository) : IQueryHandler<GetEmployeeListQuery, Result<EmployeeListResponse>>
    {
        private readonly IEmployeeRepository _employeeRepository = employeeRepository;

        public async Task<Result<EmployeeListResponse>> Handle(GetEmployeeListQuery query, CancellationToken cancellationToken = default)
        {
            // Obter funcionários paginados
            var employees = await _employeeRepository.GetAllAsync(query.Page, query.PageSize, cancellationToken);

            // Obter contagem total
            var count = await _employeeRepository.GetTotalCountAsync(cancellationToken);

            // Mapear resultados
            var mappedEmployees = employees.Select(e => new EmployeeListItemResponse(
                Id: e.Id,
                FullName: e.Name.FullName,
                Email: e.Email.Value,
                Document: e.Document.Value,
                Position: e.Position,
                Salary: e.Salary.Amount,
                IsActive: e.IsActive
            ));

            var response = new EmployeeListResponse(
                Employees: mappedEmployees,
                TotalCount: count,
                Page: query.Page,
                PageSize: query.PageSize
            );

            return Result.Success(response);
        }
    }
}