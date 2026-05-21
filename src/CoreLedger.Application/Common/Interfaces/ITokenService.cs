using CoreLedger.Domain.Users;

namespace CoreLedger.Application.Common.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    (string token, DateTime expiresAt) GenerateRefreshToken();
    Guid? GetUserIdFromExpiredToken(string token);
}
