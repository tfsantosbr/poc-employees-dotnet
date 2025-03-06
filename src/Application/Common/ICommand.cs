using Domain.Common;

namespace Application.Common
{
    public interface ICommand<TResult>
    {
    }
    
    public interface ICommand : ICommand<Result>
    {
    }
}
