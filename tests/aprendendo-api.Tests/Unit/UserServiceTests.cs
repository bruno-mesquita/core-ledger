using aprendendo_api.Application.DTOs;
using aprendendo_api.Application.Interfaces;
using aprendendo_api.Application.Services;
using aprendendo_api.Domain.Entities;
using aprendendo_api.Domain.Enums;
using aprendendo_api.Domain.Interfaces;
using Moq;

namespace aprendendo_api.Tests.Unit;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _repo = new();
    private readonly Mock<IPasswordHasher> _hasher = new();
    private readonly UserService _sut;

    public UserServiceTests()
    {
        _sut = new UserService(_repo.Object, _hasher.Object);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsUserResponse()
    {
        var user = User.Create("test@example.com", "hash");
        _repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);

        var result = await _sut.GetByIdAsync(1);

        Assert.Equal("test@example.com", result.Email);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistentId_ThrowsKeyNotFoundException()
    {
        _repo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.GetByIdAsync(99));
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllUsers()
    {
        var users = new[] { User.Create("a@x.com", "h"), User.Create("b@x.com", "h") };
        _repo.Setup(r => r.GetAllAsync()).ReturnsAsync(users);

        var result = await _sut.GetAllAsync();

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task UpdateAsync_WithEmailChange_UpdatesEmail()
    {
        var user = User.Create("old@example.com", "hash");
        _repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);

        var result = await _sut.UpdateAsync(1, new UpdateUserRequest("new@example.com", null, null));

        Assert.Equal("new@example.com", result.Email);
        _repo.Verify(r => r.UpdateAsync(user), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithPasswordChange_HashesAndUpdates()
    {
        var user = User.Create("test@example.com", "old-hash");
        _repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);
        _hasher.Setup(h => h.Hash("new-pass")).Returns("new-hash");

        await _sut.UpdateAsync(1, new UpdateUserRequest(null, "new-pass", null));

        Assert.Equal("new-hash", user.PasswordHash);
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidRole_ThrowsArgumentException()
    {
        var user = User.Create("test@example.com", "hash");
        _repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);

        await Assert.ThrowsAsync<ArgumentException>(
            () => _sut.UpdateAsync(1, new UpdateUserRequest(null, null, "SuperAdmin")));
    }

    [Fact]
    public async Task DeleteAsync_ExistingUser_CallsRepositoryDelete()
    {
        _repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(User.Create("test@example.com", "hash"));

        await _sut.DeleteAsync(1);

        _repo.Verify(r => r.DeleteAsync(1), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_NonExistentUser_ThrowsKeyNotFoundException()
    {
        _repo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.DeleteAsync(99));
    }

    [Fact]
    public async Task CreateAsync_WithInvalidRole_ThrowsArgumentException()
    {
        _repo.Setup(r => r.ExistsByEmailAsync(It.IsAny<string>())).ReturnsAsync(false);

        await Assert.ThrowsAsync<ArgumentException>(
            () => _sut.CreateAsync(new CreateUserRequest("test@example.com", "pass", "God")));
    }

    [Fact]
    public async Task CreateAsync_DuplicateEmail_ThrowsInvalidOperationException()
    {
        _repo.Setup(r => r.ExistsByEmailAsync("dup@example.com")).ReturnsAsync(true);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _sut.CreateAsync(new CreateUserRequest("dup@example.com", "pass", "User")));
    }
}
