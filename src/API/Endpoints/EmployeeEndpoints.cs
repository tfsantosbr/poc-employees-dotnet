using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common;
using Application.Employees.Commands;
using Application.Employees.Models.Responses;
using Application.Employees.Queries;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace API.Endpoints
{
    public static class EmployeeEndpoints
    {
        public static void MapEmployeeEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/employees").WithTags("Employees");

            // GET: /api/employees
            group.MapGet("/", GetEmployees)
                .WithName("GetEmployees")
                .WithOpenApi()
                .Produces<EmployeeListResponse>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status400BadRequest);

            // GET: /api/employees/{id}
            group.MapGet("/{id}", GetEmployeeById)
                .WithName("GetEmployeeById")
                .WithOpenApi()
                .Produces<EmployeeResponse>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);

            // POST: /api/employees
            group.MapPost("/", CreateEmployee)
                .WithName("CreateEmployee")
                .WithOpenApi()
                .Produces<EmployeeResponse>(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest);

            // PUT: /api/employees/{id}
            group.MapPut("/{id}", UpdateEmployee)
                .WithName("UpdateEmployee")
                .WithOpenApi()
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status404NotFound);

            // DELETE: /api/employees/{id}
            group.MapDelete("/{id}", DeleteEmployee)
                .WithName("DeleteEmployee")
                .WithOpenApi()
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status404NotFound);

            // POST: /api/employees/{id}/addresses
            group.MapPost("/{id}/addresses", AddEmployeeAddress)
                .WithName("AddEmployeeAddress")
                .WithOpenApi()
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status404NotFound);
        }

        private static async Task<IResult> GetEmployees(
            int page,
            int pageSize,
            IQueryHandler<GetEmployeeListQuery, Domain.Common.Result<EmployeeListResponse>> handler,
            CancellationToken cancellationToken)
        {
            page = page <= 0 ? 1 : page;
            pageSize = pageSize <= 0 ? 10 : Math.Min(pageSize, 100);

            var query = new GetEmployeeListQuery { Page = page, PageSize = pageSize };
            var result = await handler.Handle(query, cancellationToken);

            if (result.IsFailure)
                return Results.BadRequest(new { errors = result.Errors });

            return Results.Ok(result.Value);
        }

        private static async Task<IResult> GetEmployeeById(
            Guid id,
            IQueryHandler<GetEmployeeByIdQuery, Domain.Common.Result<EmployeeResponse>> handler,
            CancellationToken cancellationToken)
        {
            var query = new GetEmployeeByIdQuery(id);
            var result = await handler.Handle(query, cancellationToken);

            if (result.IsFailure)
                return Results.NotFound(new { errors = result.Errors });

            return Results.Ok(result.Value);
        }

        private static async Task<IResult> CreateEmployee(
            CreateEmployeeCommand command,
            ICommandHandler<CreateEmployeeCommand, Domain.Common.Result<EmployeeResponse>> handler,
            CancellationToken cancellationToken)
        {
            var result = await handler.Handle(command, cancellationToken);

            if (result.IsFailure)
                return Results.BadRequest(new { errors = result.Errors });

            return Results.Created($"/api/employees/{result.Value.Id}", result.Value);
        }

        private static async Task<IResult> UpdateEmployee(
            Guid id,
            UpdateEmployeeCommand command,
            ICommandHandler<UpdateEmployeeCommand, Domain.Common.Result> handler,
            CancellationToken cancellationToken)
        {
            if (id != command.Id)
                return Results.BadRequest(new { errors = new[] { "ID da rota não corresponde ao ID do funcionário" } });

            var result = await handler.Handle(command, cancellationToken);

            if (result.IsFailure)
                return Results.BadRequest(new { errors = result.Errors });

            return Results.NoContent();
        }

        private static async Task<IResult> DeleteEmployee(
            Guid id,
            ICommandHandler<DeleteEmployeeCommand, Domain.Common.Result> handler,
            CancellationToken cancellationToken)
        {
            var command = new DeleteEmployeeCommand(id);
            var result = await handler.Handle(command, cancellationToken);

            if (result.IsFailure)
                return Results.BadRequest(new { errors = result.Errors });

            return Results.NoContent();
        }

        private static async Task<IResult> AddEmployeeAddress(
            Guid id,
            AddEmployeeAddressCommand command,
            ICommandHandler<AddEmployeeAddressCommand, Domain.Common.Result> handler,
            CancellationToken cancellationToken)
        {
            if (id != command.EmployeeId)
                return Results.BadRequest(new { errors = new[] { "ID da rota não corresponde ao ID do funcionário" } });

            var result = await handler.Handle(command, cancellationToken);

            if (result.IsFailure)
                return Results.BadRequest(new { errors = result.Errors });

            return Results.NoContent();
        }
    }
}
