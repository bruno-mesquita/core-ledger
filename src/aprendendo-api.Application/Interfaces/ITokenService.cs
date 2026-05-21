using aprendendo_api.Domain.Entities;

namespace aprendendo_api.Application.Interfaces;

public interface ITokenService
{
    string GenerateToken(User user);
}
