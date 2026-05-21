using aprendendo_api.Domain.Entities;
using aprendendo_api.Domain.Enums;

namespace aprendendo_api.Tests.Unit;

public class UserEntityTests
{
    [Fact]
    public void Create_WithValidInputs_ReturnsUserWithCorrectProperties()
    {
        var user = User.Create("test@example.com", "hash123", Role.Admin);

        Assert.Equal("test@example.com", user.Email);
        Assert.Equal("hash123", user.PasswordHash);
        Assert.Equal(Role.Admin, user.Role);
    }

    [Fact]
    public void Create_DefaultRole_IsUser()
    {
        var user = User.Create("test@example.com", "hash123");
        Assert.Equal(Role.User, user.Role);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidEmail_ThrowsArgumentException(string? email)
    {
        Assert.ThrowsAny<ArgumentException>(() => User.Create(email!, "hash123"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidPasswordHash_ThrowsArgumentException(string? hash)
    {
        Assert.ThrowsAny<ArgumentException>(() => User.Create("test@example.com", hash!));
    }

    [Fact]
    public void UpdateEmail_WithValidEmail_SetsEmail()
    {
        var user = User.Create("old@example.com", "hash");
        user.UpdateEmail("new@example.com");
        Assert.Equal("new@example.com", user.Email);
    }

    [Fact]
    public void UpdateEmail_WithBlankEmail_ThrowsArgumentException()
    {
        var user = User.Create("test@example.com", "hash");
        Assert.Throws<ArgumentException>(() => user.UpdateEmail(""));
    }

    [Fact]
    public void UpdateRole_SetsRole()
    {
        var user = User.Create("test@example.com", "hash", Role.User);
        user.UpdateRole(Role.Admin);
        Assert.Equal(Role.Admin, user.Role);
    }
}
