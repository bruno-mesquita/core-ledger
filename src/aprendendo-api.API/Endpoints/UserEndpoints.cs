using aprendendo_api.Application.DTOs;
using aprendendo_api.Application.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace aprendendo_api.API.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/users").RequireAuthorization();

        group.MapGet("/", async (IUserService service) =>
            await service.GetAllAsync());

        group.MapGet("/{id}", async Task<Results<Ok<UserResponse>, NotFound>> (int id, IUserService service) =>
        {
            try
            {
                var user = await service.GetByIdAsync(id);
                return TypedResults.Ok(user);
            }
            catch (KeyNotFoundException)
            {
                return TypedResults.NotFound();
            }
        });

        group.MapPost("/", async Task<Results<Created<UserResponse>, Conflict<string>, BadRequest<string>>> (
            CreateUserRequest request, IUserService service) =>
        {
            try
            {
                var created = await service.CreateAsync(request);
                return TypedResults.Created($"/users/{created.Id}", created);
            }
            catch (InvalidOperationException ex)
            {
                return TypedResults.Conflict(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return TypedResults.BadRequest(ex.Message);
            }
        }).RequireAuthorization(p => p.RequireRole("Admin"));

        group.MapPut("/{id}", async Task<Results<Ok<UserResponse>, NotFound, BadRequest<string>>> (
            int id, UpdateUserRequest request, IUserService service) =>
        {
            try
            {
                var updated = await service.UpdateAsync(id, request);
                return TypedResults.Ok(updated);
            }
            catch (KeyNotFoundException)
            {
                return TypedResults.NotFound();
            }
            catch (ArgumentException ex)
            {
                return TypedResults.BadRequest(ex.Message);
            }
        });

        group.MapDelete("/{id}", async Task<Results<NoContent, NotFound>> (int id, IUserService service) =>
        {
            try
            {
                await service.DeleteAsync(id);
                return TypedResults.NoContent();
            }
            catch (KeyNotFoundException)
            {
                return TypedResults.NotFound();
            }
        }).RequireAuthorization(p => p.RequireRole("Admin"));
    }
}
