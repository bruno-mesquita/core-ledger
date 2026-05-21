using CoreLedger.Application.Auth.DTOs;
using CoreLedger.Application.Common.Interfaces;
using CoreLedger.Domain.Users;
using CoreLedger.SharedKernel;
using MediatR;

namespace CoreLedger.Application.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler(
    IUserRepository userRepository,
    ITokenService tokenService,
    IUnitOfWork unitOfWork) : IRequestHandler<RefreshTokenCommand, Result<AuthResponse>>
{
    public async Task<Result<AuthResponse>> Handle(RefreshTokenCommand request, CancellationToken ct)
    {
        var user = await userRepository.GetByRefreshTokenAsync(request.Token, ct);
        if (user is null)
            return Result.Failure<AuthResponse>("Invalid refresh token.");

        var activeToken = user.GetActiveRefreshToken(request.Token);
        if (activeToken is null)
            return Result.Failure<AuthResponse>("Refresh token is expired or revoked.");

        user.RevokeRefreshToken(request.Token);

        var accessToken = tokenService.GenerateAccessToken(user);
        var (newRefreshTokenStr, expiresAt) = tokenService.GenerateRefreshToken();
        user.AddRefreshToken(newRefreshTokenStr, expiresAt);

        await userRepository.UpdateAsync(user, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(new AuthResponse(
            accessToken,
            newRefreshTokenStr,
            user.Email,
            user.Role.ToString()));
    }
}
