using System.Reflection;
using Application.Common;
using Application.Employees.Commands;
using Application.Employees.Handlers;
using Application.Employees.Queries;
using Application.Employees.Validators;
using Domain.Repositories;
using FluentValidation;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Database
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

            // Repositories
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<AppDbContext>());

            // Validators
            services.AddScoped<IValidator<CreateEmployeeCommand>, CreateEmployeeCommandValidator>();
            services.AddScoped<IValidator<UpdateEmployeeCommand>, UpdateEmployeeCommandValidator>();
            services.AddScoped<IValidator<AddEmployeeAddressCommand>, AddEmployeeAddressCommandValidator>();

            // Command Handlers
            services.AddScoped<ICommandHandler<CreateEmployeeCommand, Domain.Common.Result<Application.Employees.Models.Responses.EmployeeResponse>>, CreateEmployeeCommandHandler>();
            services.AddScoped<ICommandHandler<UpdateEmployeeCommand, Domain.Common.Result>, UpdateEmployeeCommandHandler>();
            services.AddScoped<ICommandHandler<DeleteEmployeeCommand, Domain.Common.Result>, DeleteEmployeeCommandHandler>();
            services.AddScoped<ICommandHandler<AddEmployeeAddressCommand, Domain.Common.Result>, AddEmployeeAddressCommandHandler>();

            // Query Handlers
            services.AddScoped<IQueryHandler<GetEmployeeByIdQuery, Domain.Common.Result<Application.Employees.Models.Responses.EmployeeResponse>>, GetEmployeeByIdQueryHandler>();
            services.AddScoped<IQueryHandler<GetEmployeeListQuery, Domain.Common.Result<Application.Employees.Models.Responses.EmployeeListResponse>>, GetEmployeeListQueryHandler>();

            return services;
        }
    }
}
