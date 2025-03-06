using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Entities;

namespace Domain.Repositories
{
    public interface IEmployeeRepository
    {
        Task<Result<Employee>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Result<IEnumerable<Employee>>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default);
        Task<Result<int>> GetTotalCountAsync(CancellationToken cancellationToken = default);
        Task<Result<Employee>> AddAsync(Employee employee, CancellationToken cancellationToken = default);
        Task<Result> UpdateAsync(Employee employee, CancellationToken cancellationToken = default);
        Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Result<bool>> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Result<bool>> EmailExistsAsync(string email, Guid? excludeId = null, CancellationToken cancellationToken = default);
        Task<Result<bool>> DocumentExistsAsync(string document, Guid? excludeId = null, CancellationToken cancellationToken = default);
    }
}
