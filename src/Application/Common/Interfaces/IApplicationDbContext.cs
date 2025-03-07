using System.Threading;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Employee> Employees { get; }
        DbSet<EmployeeAddress> EmployeeAddresses { get; }
        
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}