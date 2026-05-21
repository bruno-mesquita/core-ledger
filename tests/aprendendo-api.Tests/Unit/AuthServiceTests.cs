using aprendendo_api.Application.DTOs;
using aprendendo_api.Application.Interfaces;
using aprendendo_api.Application.Services;
using aprendendo_api.Domain.Entities;
using aprendendo_api.Domain.Interfaces;
using Moq;

namespace aprendendo_api.Tests.Unit;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _repo = new();
    private readonly Mock<IPasswordHasher> _hasher = new();
    private readonly Mock<ITokenService> _token = new();
    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        _sut = new AuthService(_repo.Object, _hasher.Object, _token.Object);
    }

    [Fact]
    public async Task RegisterAsync_NewEmail_ReturnsAuthResponse()
    {
        _repo.Setup(r => r.ExistsByEmailAsync("test@example.com")).ReturnsAsync(false);
        _hasher.Setup(h => h.Hash("pass")).Returns("hashed");
        _repo.Setup(r => r.AddAsync(It.IsAny<User>()))
            .ReturnsAsync((User u) => u);
        _token.Setup(t => t.GenerateToken(It.IsAny<User>())).Returns("jwt-token");

        var result = await _sut.RegisterAsync(new RegisterRequest("test@example.com", "pass"));

        Assert.Equal("jwt-token", result.Token);
        Assert.Equal("test@example.com", result.Email);
        _repo.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_DuplicateEmail_ThrowsInvalidOperationException()
    {
        _repo.Setup(r => r.ExistsByEmailAsync("test@example.com")).ReturnsAsync(true);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _sut.RegisterAsync(new RegisterRequest("test@example.com", "pass")));
    }

    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnsAuthResponse()
    {
        var user = User.Create("test@example.com", "hashed");
        _repo.Setup(r => r.GetByEmailAsync("test@example.com")).ReturnsAsync(user);
        _hasher.Setup(h => h.Verify("pass", "hashed")).Returns(true);
        _token.Setup(t => t.GenerateToken(user)).Returns("jwt-token");

        var result = await _sut.LoginAsync(new LoginRequest("test@example.com", "pass"));

        Assert.Equal("jwt-token", result.Token);
    }

    [Fact]
    public async Task LoginAsync_UserNotFound_ThrowsUnauthorizedAccessException()
    {
        _repo.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _sut.LoginAsync(new LoginRequest("nobody@example.com", "pass")));
    }

    [Fact]
    public async Task LoginAsync_WrongPassword_ThrowsUnauthorizedAccessException()
    {
        var user = User.Create("test@example.com", "hashed");
        _repo.Setup(r => r.GetByEmailAsync("test@example.com")).ReturnsAsync(user);
        _hasher.Setup(h => h.Verify("wrong", "hashed")).Returns(false);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _sut.LoginAsync(new LoginRequest("test@example.com", "wrong")));
    }
}
