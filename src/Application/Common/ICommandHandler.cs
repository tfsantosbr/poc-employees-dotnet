using System.Threading;
using System.Threading.Tasks;

namespace Application.Common
{
    public interface ICommandHandler<in TCommand, TResult> where TCommand : ICommand<TResult>
    {
        Task<TResult> Handle(TCommand command, CancellationToken cancellationToken = default);
    }
}
