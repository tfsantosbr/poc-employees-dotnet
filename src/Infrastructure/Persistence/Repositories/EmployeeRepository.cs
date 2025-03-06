using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly AppDbContext _dbContext;

        public EmployeeRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<Employee>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            try
            {
                var employee = await _dbContext.Employees
                    .AsNoTracking()
                    .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

                if (employee == null)
                    return Result.Failure<Employee>("Funcionário não encontrado");

                return Result.Success(employee);
            }
            catch (Exception ex)
            {
                return Result.Failure<Employee>($"Erro ao buscar funcionário: {ex.Message}");
            }
        }

        public async Task<Result<IEnumerable<Employee>>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            try
            {
                var employees = await _dbContext.Employees
                    .AsNoTracking()
                    .OrderByDescending(e => e.UpdatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync(cancellationToken);

                return Result.Success<IEnumerable<Employee>>(employees);
            }
            catch (Exception ex)
            {
                return Result.Failure<IEnumerable<Employee>>($"Erro ao buscar lista de funcionários: {ex.Message}");
            }
        }

        public async Task<Result<int>> GetTotalCountAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var count = await _dbContext.Employees.CountAsync(cancellationToken);
                return Result.Success(count);
            }
            catch (Exception ex)
            {
                return Result.Failure<int>($"Erro ao obter contagem de funcionários: {ex.Message}");
            }
        }

        public async Task<Result<Employee>> AddAsync(Employee employee, CancellationToken cancellationToken = default)
        {
            try
            {
                await _dbContext.Employees.AddAsync(employee, cancellationToken);
                return Result.Success(employee);
            }
            catch (Exception ex)
            {
                return Result.Failure<Employee>($"Erro ao adicionar funcionário: {ex.Message}");
            }
        }

        public async Task<Result> UpdateAsync(Employee employee, CancellationToken cancellationToken = default)
        {
            try
            {
                _dbContext.Employees.Update(employee);
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"Erro ao atualizar funcionário: {ex.Message}");
            }
        }

        public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            try
            {
                var employee = await _dbContext.Employees.FindAsync(new object[] { id }, cancellationToken);
                if (employee == null)
                    return Result.Failure("Funcionário não encontrado");

                employee.Deactivate();
                _dbContext.Employees.Update(employee);
                
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"Erro ao excluir funcionário: {ex.Message}");
            }
        }

        public async Task<Result<bool>> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            try
            {
                var exists = await _dbContext.Employees.AnyAsync(e => e.Id == id, cancellationToken);
                return Result.Success(exists);
            }
            catch (Exception ex)
            {
                return Result.Failure<bool>($"Erro ao verificar existência do funcionário: {ex.Message}");
            }
        }

        public async Task<Result<bool>> EmailExistsAsync(string email, Guid? excludeId = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var query = _dbContext.Employees.AsQueryable();
                
                if (excludeId.HasValue)
                    query = query.Where(e => e.Id != excludeId.Value);

                // Como estamos usando um Value Object, precisamos usar a expressão JSON para buscar
                var exists = await query.AnyAsync(e => e.Email.Value == email, cancellationToken);
                return Result.Success(exists);
            }
            catch (Exception ex)
            {
                return Result.Failure<bool>($"Erro ao verificar existência do email: {ex.Message}");
            }
        }

        public async Task<Result<bool>> DocumentExistsAsync(string document, Guid? excludeId = null, CancellationToken cancellationToken = default)
        {
            try
            {
                // Remove caracteres não numéricos
                var numericDocument = System.Text.RegularExpressions.Regex.Replace(document, @"[^\d]", "");

                var query = _dbContext.Employees.AsQueryable();
                
                if (excludeId.HasValue)
                    query = query.Where(e => e.Id != excludeId.Value);

                // Como estamos usando um Value Object, precisamos usar a expressão JSON para buscar
                var exists = await query.AnyAsync(e => e.Document.Value == numericDocument, cancellationToken);
                return Result.Success(exists);
            }
            catch (Exception ex)
            {
                return Result.Failure<bool>($"Erro ao verificar existência do documento: {ex.Message}");
            }
        }
    }
}
