using CoreLedger.Application.Auth.Commands.Login;
using CoreLedger.Application.Common.Interfaces;
using CoreLedger.Domain.Users;

namespace CoreLedger.UnitTests.Application.Auth;

public class LoginCommandHandlerTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
    private readonly ITokenService _tokenService = Substitute.For<ITokenService>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    private LoginCommandHandler CreateHandler() =>
        new(_userRepository, _passwordHasher, _tokenService, _unitOfWork);

    [Fact]
    public async Task Handle_ValidCredentials_ReturnsSuccess()
    {
        var user = User.Create("test@test.com", "hash");
        _userRepository.GetByEmailAsync("test@test.com").Returns(user);
        _passwordHasher.Verify("Password1!", "hash").Returns(true);
        _tokenService.GenerateAccessToken(user).Returns("access");
        _tokenService.GenerateRefreshToken().Returns(("refresh", DateTime.UtcNow.AddDays(7)));

        var result = await CreateHandler().Handle(
            new LoginCommand("test@test.com", "Password1!"),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.AccessToken.Should().Be("access");
    }

    [Fact]
    public async Task Handle_UserNotFound_ReturnsFailure()
    {
        _userRepository.GetByEmailAsync("notfound@test.com").Returns((User?)null);

        var result = await CreateHandler().Handle(
            new LoginCommand("notfound@test.com", "pass"),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WrongPassword_ReturnsFailure()
    {
        var user = User.Create("test@test.com", "hash");
        _userRepository.GetByEmailAsync("test@test.com").Returns(user);
        _passwordHasher.Verify("wrong", "hash").Returns(false);

        var result = await CreateHandler().Handle(
            new LoginCommand("test@test.com", "wrong"),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Invalid credentials");
    }
}
