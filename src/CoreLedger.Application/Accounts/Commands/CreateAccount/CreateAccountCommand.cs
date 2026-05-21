using CoreLedger.Application.Accounts.DTOs;
using CoreLedger.SharedKernel;
using MediatR;

namespace CoreLedger.Application.Accounts.Commands.CreateAccount;

public record CreateAccountCommand(Guid OwnerId) : IRequest<Result<AccountResponse>>;
