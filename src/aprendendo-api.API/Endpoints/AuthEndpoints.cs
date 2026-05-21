using aprendendo_api.Application.DTOs;
using aprendendo_api.Application.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace aprendendo_api.API.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/auth");

        group.MapPost("/register", async Task<Results<Created<AuthResponse>, Conflict<string>, BadRequest<string>>> (
            RegisterRequest request, IAuthService service) =>
        {
            try
            {
                var response = await service.RegisterAsync(request);
                return TypedResults.Created($"/users", response);
            }
            catch (InvalidOperationException ex)
            {
                return TypedResults.Conflict(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return TypedResults.BadRequest(ex.Message);
            }
        });

        group.MapPost("/login", async Task<Results<Ok<AuthResponse>, UnauthorizedHttpResult>> (
            LoginRequest request, IAuthService service) =>
        {
            try
            {
                var response = await service.LoginAsync(request);
                return TypedResults.Ok(response);
            }
            catch (UnauthorizedAccessException)
            {
                return TypedResults.Unauthorized();
            }
        });
    }
}
