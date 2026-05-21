using CoreLedger.Application.Auth.DTOs;
using CoreLedger.Application.Common.Interfaces;
using CoreLedger.Domain.Users;
using CoreLedger.SharedKernel;
using MediatR;

namespace CoreLedger.Application.Auth.Commands.Login;

public class LoginCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    ITokenService tokenService,
    IUnitOfWork unitOfWork) : IRequestHandler<LoginCommand, Result<AuthResponse>>
{
    public async Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken ct)
    {
        var user = await userRepository.GetByEmailAsync(request.Email, ct);
        if (user is null || !passwordHasher.Verify(request.Password, user.PasswordHash))
            return Result.Failure<AuthResponse>("Invalid credentials.");

        var accessToken = tokenService.GenerateAccessToken(user);
        var (refreshTokenStr, expiresAt) = tokenService.GenerateRefreshToken();
        user.AddRefreshToken(refreshTokenStr, expiresAt);

        await userRepository.UpdateAsync(user, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(new AuthResponse(
            accessToken,
            refreshTokenStr,
            user.Email,
            user.Role.ToString()));
    }
}
