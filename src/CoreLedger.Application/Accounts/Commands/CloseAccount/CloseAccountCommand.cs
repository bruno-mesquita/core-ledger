using CoreLedger.SharedKernel;
using MediatR;

namespace CoreLedger.Application.Accounts.Commands.CloseAccount;

public record CloseAccountCommand(Guid AccountId, Guid RequesterId) : IRequest<Result>;
