using CoreLedger.Application.Auth.DTOs;
using CoreLedger.SharedKernel;
using MediatR;

namespace CoreLedger.Application.Auth.Commands.Login;

public record LoginCommand(string Email, string Password) : IRequest<Result<AuthResponse>>;
