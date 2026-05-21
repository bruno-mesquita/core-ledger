using aprendendo_api.Application.DTOs;
using aprendendo_api.Application.Interfaces;
using aprendendo_api.Domain.Entities;
using aprendendo_api.Domain.Enums;
using aprendendo_api.Domain.Interfaces;

namespace aprendendo_api.Application.Services;

public class AuthService(IUserRepository repository, IPasswordHasher passwordHasher, ITokenService tokenService) : IAuthService
{
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        if (await repository.ExistsByEmailAsync(request.Email))
            throw new InvalidOperationException("Email already registered.");

        var hash = passwordHasher.Hash(request.Password);
        var user = User.Create(request.Email, hash, Role.User);
        var created = await repository.AddAsync(user);
        var token = tokenService.GenerateToken(created);

        return new AuthResponse(token, created.Email, created.Role.ToString());
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await repository.GetByEmailAsync(request.Email)
            ?? throw new UnauthorizedAccessException("Invalid credentials.");

        if (!passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials.");

        var token = tokenService.GenerateToken(user);
        return new AuthResponse(token, user.Email, user.Role.ToString());
    }
}
