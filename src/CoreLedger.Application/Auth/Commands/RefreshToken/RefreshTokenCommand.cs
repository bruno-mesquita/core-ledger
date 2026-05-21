using CoreLedger.Application.Auth.DTOs;
using CoreLedger.SharedKernel;
using MediatR;

namespace CoreLedger.Application.Auth.Commands.RefreshToken;

public record RefreshTokenCommand(string Token) : IRequest<Result<AuthResponse>>;
