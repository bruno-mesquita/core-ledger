using CoreLedger.SharedKernel;

namespace CoreLedger.Domain.Users;

public class User : AggregateRoot
{
    private readonly List<RefreshToken> _refreshTokens = [];

    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public Role Role { get; private set; }
    public IReadOnlyList<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

    private User() { }

    public static User Create(string email, string passwordHash, Role role = Role.Customer) =>
        new()
        {
            Id = Guid.NewGuid(),
            Email = email,
            PasswordHash = passwordHash,
            Role = role
        };

    public void UpdatePasswordHash(string hash) => PasswordHash = hash;

    public RefreshToken AddRefreshToken(string token, DateTime expiresAt)
    {
        var refreshToken = RefreshToken.Create(Id, token, expiresAt);
        _refreshTokens.Add(refreshToken);
        return refreshToken;
    }

    public RefreshToken? GetActiveRefreshToken(string token) =>
        _refreshTokens.FirstOrDefault(t => t.Token == token && t.IsActive);

    public void RevokeRefreshToken(string token) =>
        _refreshTokens.FirstOrDefault(t => t.Token == token)?.Revoke();
}
