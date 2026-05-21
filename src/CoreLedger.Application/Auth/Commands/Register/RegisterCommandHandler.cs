using CoreLedger.Application.Auth.DTOs;
using CoreLedger.Application.Common.Interfaces;
using CoreLedger.Domain.Users;
using CoreLedger.SharedKernel;
using MediatR;

namespace CoreLedger.Application.Auth.Commands.Register;

public class RegisterCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    ITokenService tokenService,
    IUnitOfWork unitOfWork) : IRequestHandler<RegisterCommand, Result<AuthResponse>>
{
    public async Task<Result<AuthResponse>> Handle(RegisterCommand request, CancellationToken ct)
    {
        if (await userRepository.ExistsByEmailAsync(request.Email, ct))
            return Result.Failure<AuthResponse>("Email already registered.");

        var passwordHash = passwordHasher.Hash(request.Password);
        var user = User.Create(request.Email, passwordHash);

        var accessToken = tokenService.GenerateAccessToken(user);
        var (refreshTokenStr, expiresAt) = tokenService.GenerateRefreshToken();
        user.AddRefreshToken(refreshTokenStr, expiresAt);

        await userRepository.AddAsync(user, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(new AuthResponse(
            accessToken,
            refreshTokenStr,
            user.Email,
            user.Role.ToString()));
    }
}
