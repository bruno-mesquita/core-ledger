using aprendendo_api.Application.DTOs;

namespace aprendendo_api.Application.Services;

public interface IUserService
{
    Task<UserResponse> GetByIdAsync(int id);
    Task<IEnumerable<UserResponse>> GetAllAsync();
    Task<UserResponse> CreateAsync(CreateUserRequest request);
    Task<UserResponse> UpdateAsync(int id, UpdateUserRequest request);
    Task DeleteAsync(int id);
}
