using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class EmployeeRepository(IApplicationDbContext dbContext) : IEmployeeRepository
    {

        public async Task<Employee> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await dbContext.Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Employee>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            return await dbContext.Employees
                .AsNoTracking()
                .OrderByDescending(e => e.UpdatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<int> GetTotalCountAsync(CancellationToken cancellationToken = default)
        {
            return await dbContext.Employees.CountAsync(cancellationToken);
        }

        public async Task<Employee> AddAsync(Employee employee, CancellationToken cancellationToken = default)
        {
            await dbContext.Employees.AddAsync(employee, cancellationToken);
            return employee;
        }

        public Task UpdateAsync(Employee employee, CancellationToken cancellationToken = default)
        {
            dbContext.Employees.Update(employee);
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var employee = await dbContext.Employees.FindAsync(new object[] { id }, cancellationToken);
            if (employee != null)
            {
                employee.Deactivate();
                dbContext.Employees.Update(employee);
            }
        }

        public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await dbContext.Employees.AnyAsync(e => e.Id == id, cancellationToken);
        }

        public async Task<bool> EmailExistsAsync(string email, Guid? excludeId = null, CancellationToken cancellationToken = default)
        {
            var query = dbContext.Employees.AsQueryable();

            if (excludeId.HasValue)
                query = query.Where(e => e.Id != excludeId.Value);

            return await query.AnyAsync(e => e.Email.Value == email, cancellationToken);
        }

        public async Task<bool> DocumentExistsAsync(string document, Guid? excludeId = null, CancellationToken cancellationToken = default)
        {
            // Remove caracteres não numéricos
            var numericDocument = System.Text.RegularExpressions.Regex.Replace(document, @"[^\d]", "");

            var query = dbContext.Employees.AsQueryable();

            if (excludeId.HasValue)
                query = query.Where(e => e.Id != excludeId.Value);

            return await query.AnyAsync(e => e.Document.Value == numericDocument, cancellationToken);
        }
    }
}
