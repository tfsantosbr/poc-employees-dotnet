using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common;
using Application.Employees.Models.Responses;
using Domain.Common;
using Domain.Repositories;

namespace Application.Employees.Queries
{
    public class GetEmployeeListQueryHandler(IEmployeeRepository employeeRepository) : IQueryHandler<GetEmployeeListQuery, Result<EmployeeListResponse>>
    {
        public async Task<Result<EmployeeListResponse>> Handle(GetEmployeeListQuery query, CancellationToken cancellationToken = default)
        {
            // Obter funcionários paginados
            var employees = await employeeRepository.GetAllAsync(
                query.Page,
                query.PageSize,
                cancellationToken);

            // Obter contagem total
            var totalCount = await employeeRepository.GetTotalCountAsync(cancellationToken);

            // Mapear resultados
            var employeeItems = employees.Select(e => new EmployeeListItemResponse(
                Id: e.Id,
                FullName: e.Name.FullName,
                Email: e.Email.Value,
                Document: e.Document.Value,
                Position: e.Position,
                Salary: e.Salary.Amount,
                IsActive: e.IsActive
            )).ToList();

            var response = new EmployeeListResponse(
                Employees: employeeItems,
                TotalCount: totalCount,
                Page: query.Page,
                PageSize: query.PageSize
            );

            return Result.Success(response);
        }
    }
}
