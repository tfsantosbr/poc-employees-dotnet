using System;
using System.Threading;
using System.Threading.Tasks;
using Domain.Common;

namespace Domain.Repositories
{
    public interface IUnitOfWork
    {
        Task<Result> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
