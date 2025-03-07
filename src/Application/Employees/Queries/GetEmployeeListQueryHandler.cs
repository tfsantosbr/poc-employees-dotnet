using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common;
using Application.Common.Interfaces;
using Application.Employees.Models.Responses;
using Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Application.Employees.Queries
{
    public class GetEmployeeListQueryHandler(IApplicationDbContext dbContext) : IQueryHandler<GetEmployeeListQuery, Result<EmployeeListResponse>>
    {
        private readonly IApplicationDbContext _dbContext = dbContext;

        public async Task<Result<EmployeeListResponse>> Handle(GetEmployeeListQuery query, CancellationToken cancellationToken = default)
        {
            // Obter funcionários paginados
            var employees = await _dbContext.Employees
                .AsNoTracking()
                .OrderByDescending(e => e.UpdatedAt)
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync(cancellationToken);

            // Obter contagem total
            var totalCount = await _dbContext.Employees.CountAsync(cancellationToken);

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
