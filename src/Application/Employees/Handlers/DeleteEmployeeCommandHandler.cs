using System.Threading;
using System.Threading.Tasks;
using Application.Common;
using Application.Employees.Commands;
using Domain.Common;
using Domain.Repositories;
using FluentValidation;

namespace Application.Employees.Handlers
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
            var existsResult = await _employeeRepository.ExistsAsync(command.Id, cancellationToken);
            if (existsResult.IsFailure)
                return Result.Failure(existsResult.Errors);

            if (!existsResult.Value)
                return Result.Failure("Funcionário não encontrado");

            // Excluir o funcionário
            var result = await _employeeRepository.DeleteAsync(command.Id, cancellationToken);
            if (result.IsFailure)
                return Result.Failure(result.Errors);

            // Salvar as alterações
            var saveResult = await _unitOfWork.SaveChangesAsync(cancellationToken);
            if (saveResult.IsFailure)
                return Result.Failure(saveResult.Errors);

            return Result.Success();
        }
    }
}
