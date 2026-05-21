using aprendendo_api.Application.DTOs;
using aprendendo_api.Application.Interfaces;
using aprendendo_api.Domain.Entities;
using aprendendo_api.Domain.Enums;
using aprendendo_api.Domain.Interfaces;

namespace aprendendo_api.Application.Services;

public class UserService(IUserRepository repository, IPasswordHasher passwordHasher) : IUserService
{
    public async Task<UserResponse> GetByIdAsync(int id)
    {
        var user = await repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"User {id} not found.");
        return UserResponse.FromEntity(user);
    }

    public async Task<IEnumerable<UserResponse>> GetAllAsync()
    {
        var users = await repository.GetAllAsync();
        return users.Select(UserResponse.FromEntity);
    }

    public async Task<UserResponse> CreateAsync(CreateUserRequest request)
    {
        if (await repository.ExistsByEmailAsync(request.Email))
            throw new InvalidOperationException("Email already registered.");

        if (!Enum.TryParse<Role>(request.Role, ignoreCase: true, out var role))
            throw new ArgumentException($"Invalid role: {request.Role}.");

        var hash = passwordHasher.Hash(request.Password);
        var user = User.Create(request.Email, hash, role);
        var created = await repository.AddAsync(user);
        return UserResponse.FromEntity(created);
    }

    public async Task<UserResponse> UpdateAsync(int id, UpdateUserRequest request)
    {
        var user = await repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"User {id} not found.");

        if (request.Email is not null)
            user.UpdateEmail(request.Email);

        if (request.Password is not null)
            user.UpdatePasswordHash(passwordHasher.Hash(request.Password));

        if (request.Role is not null)
        {
            if (!Enum.TryParse<Role>(request.Role, ignoreCase: true, out var role))
                throw new ArgumentException($"Invalid role: {request.Role}.");
            user.UpdateRole(role);
        }

        await repository.UpdateAsync(user);
        return UserResponse.FromEntity(user);
    }

    public async Task DeleteAsync(int id)
    {
        if (await repository.GetByIdAsync(id) is null)
            throw new KeyNotFoundException($"User {id} not found.");
        await repository.DeleteAsync(id);
    }
}
