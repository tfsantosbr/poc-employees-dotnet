using System.Threading;
using System.Threading.Tasks;
using Application.Common;
using Domain.Common;
using Domain.Repositories;

namespace Application.Employees.Commands
{
    public class DeleteEmployeeCommandHandler : ICommandHandler<DeleteEmployeeCommand, Result>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteEmployeeCommandHandler(
            IEmployeeRepository employeeRepository,
            IUnitOfWork unitOfWork)
        {
            _employeeRepository = employeeRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(DeleteEmployeeCommand command, CancellationToken cancellationToken = default)
        {
            // Verificar se o funcionário existe
            var exists = await _employeeRepository.ExistsAsync(command.Id, cancellationToken);
            if (\!exists)
                return Result.Failure("NOT_FOUND", "Funcionário não encontrado");

            // Excluir o funcionário
            await _employeeRepository.DeleteAsync(command.Id, cancellationToken);

            // Salvar as alterações
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
