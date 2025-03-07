using System.Threading;
using System.Threading.Tasks;
using Application.Common;
using Application.Employees.Commands;
using Domain.Common;
using Domain.Repositories;

namespace Application.Employees.Handlers
{
    public class DeleteEmployeeCommandHandler(
        IEmployeeRepository employeeRepository,
        IUnitOfWork unitOfWork) : ICommandHandler<DeleteEmployeeCommand, Result>
    {
        public async Task<Result> Handle(DeleteEmployeeCommand command, CancellationToken cancellationToken = default)
        {
            // Verificar se o funcionário existe
            var exists = await employeeRepository.ExistsAsync(command.Id, cancellationToken);
            if (!exists)
                return Result.Failure("NOT_FOUND", "Funcionário não encontrado");

            // Excluir o funcionário
            await employeeRepository.DeleteAsync(command.Id, cancellationToken);

            // Salvar as alterações
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
