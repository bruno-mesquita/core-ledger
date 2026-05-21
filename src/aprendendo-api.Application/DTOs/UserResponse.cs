using aprendendo_api.Domain.Entities;

namespace aprendendo_api.Application.DTOs;

public record UserResponse(int Id, string Email, string Role)
{
    public static UserResponse FromEntity(User user) =>
        new(user.Id, user.Email, user.Role.ToString());
}
