using Application.Common;
using Application.Common.Interfaces;
using Application.Employees.Commands;
using Application.Employees.Models.Responses;
using Domain.Common;
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

            // DbContext and Repositories
            services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<AppDbContext>());
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<AppDbContext>());

            // Validators
            services.AddScoped<IValidator<CreateEmployeeCommand>, Application.Employees.Validators.CreateEmployeeCommandValidator>();
            services.AddScoped<IValidator<UpdateEmployeeCommand>, Application.Employees.Validators.UpdateEmployeeCommandValidator>();
            services.AddScoped<IValidator<AddEmployeeAddressCommand>, Application.Employees.Validators.AddEmployeeAddressCommandValidator>();

            // Command Handlers
            services.AddScoped<ICommandHandler<CreateEmployeeCommand, Result<EmployeeResponse>>, Application.Employees.Handlers.CreateEmployeeCommandHandler>();
            services.AddScoped<ICommandHandler<UpdateEmployeeCommand, Result>, Application.Employees.Handlers.UpdateEmployeeCommandHandler>();
            services.AddScoped<ICommandHandler<DeleteEmployeeCommand, Result>, Application.Employees.Handlers.DeleteEmployeeCommandHandler>();
            services.AddScoped<ICommandHandler<AddEmployeeAddressCommand, Result>, Application.Employees.Handlers.AddEmployeeAddressCommandHandler>();

            // Query Handlers
            services.AddScoped<IQueryHandler<Application.Employees.Queries.GetEmployeeByIdQuery, Result<EmployeeResponse>>, Application.Employees.Handlers.GetEmployeeByIdQueryHandler>();
            services.AddScoped<IQueryHandler<Application.Employees.Queries.GetEmployeeListQuery, Result<EmployeeListResponse>>, Application.Employees.Handlers.GetEmployeeListQueryHandler>();

            return services;
        }
    }
}