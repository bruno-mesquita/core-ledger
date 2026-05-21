using CoreLedger.Application.Auth.Commands.Register;
using CoreLedger.Application.Common.Interfaces;
using CoreLedger.Domain.Users;
using CoreLedger.SharedKernel;

namespace CoreLedger.UnitTests.Application.Auth;

public class RegisterCommandHandlerTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
    private readonly ITokenService _tokenService = Substitute.For<ITokenService>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    private RegisterCommandHandler CreateHandler() =>
        new(_userRepository, _passwordHasher, _tokenService, _unitOfWork);

    [Fact]
    public async Task Handle_NewEmail_ReturnsSuccessWithTokens()
    {
        _userRepository.ExistsByEmailAsync("new@test.com").Returns(false);
        _passwordHasher.Hash("Password1!").Returns("hash");
        _tokenService.GenerateAccessToken(Arg.Any<User>()).Returns("access-token");
        _tokenService.GenerateRefreshToken().Returns(("refresh-token", DateTime.UtcNow.AddDays(7)));

        var result = await CreateHandler().Handle(
            new RegisterCommand("new@test.com", "Password1!"),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.AccessToken.Should().Be("access-token");
        result.Value.Email.Should().Be("new@test.com");
        await _userRepository.Received(1).AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_DuplicateEmail_ReturnsFailure()
    {
        _userRepository.ExistsByEmailAsync("existing@test.com").Returns(true);

        var result = await CreateHandler().Handle(
            new RegisterCommand("existing@test.com", "Password1!"),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("already registered");
        await _userRepository.DidNotReceive().AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
    }
}
