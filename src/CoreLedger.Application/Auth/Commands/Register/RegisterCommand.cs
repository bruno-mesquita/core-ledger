using CoreLedger.Application.Auth.DTOs;
using CoreLedger.SharedKernel;
using MediatR;

namespace CoreLedger.Application.Auth.Commands.Register;

public record RegisterCommand(string Email, string Password) : IRequest<Result<AuthResponse>>;
