using CoreLedger.Domain.Users;

namespace CoreLedger.UnitTests.Domain;

public class UserTests
{
    [Fact]
    public void Create_ValidData_ReturnsUserWithCorrectProperties()
    {
        var user = User.Create("test@example.com", "hash123");

        user.Email.Should().Be("test@example.com");
        user.PasswordHash.Should().Be("hash123");
        user.Role.Should().Be(Role.Customer);
        user.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void Create_WithAdminRole_ReturnsAdminUser()
    {
        var user = User.Create("admin@example.com", "hash", Role.Admin);
        user.Role.Should().Be(Role.Admin);
    }

    [Fact]
    public void AddRefreshToken_AddsTokenToCollection()
    {
        var user = User.Create("test@example.com", "hash");
        var expiresAt = DateTime.UtcNow.AddDays(7);

        var token = user.AddRefreshToken("token123", expiresAt);

        user.RefreshTokens.Should().HaveCount(1);
        token.Token.Should().Be("token123");
        token.IsActive.Should().BeTrue();
    }

    [Fact]
    public void RevokeRefreshToken_RevokesMatchingToken()
    {
        var user = User.Create("test@example.com", "hash");
        user.AddRefreshToken("token123", DateTime.UtcNow.AddDays(7));

        user.RevokeRefreshToken("token123");

        user.RefreshTokens[0].IsRevoked.Should().BeTrue();
    }

    [Fact]
    public void GetActiveRefreshToken_ExpiredToken_ReturnsNull()
    {
        var user = User.Create("test@example.com", "hash");
        user.AddRefreshToken("expired", DateTime.UtcNow.AddDays(-1));

        var result = user.GetActiveRefreshToken("expired");

        result.Should().BeNull();
    }

    [Fact]
    public void UpdatePasswordHash_UpdatesHash()
    {
        var user = User.Create("test@example.com", "oldhash");
        user.UpdatePasswordHash("newhash");
        user.PasswordHash.Should().Be("newhash");
    }
}
