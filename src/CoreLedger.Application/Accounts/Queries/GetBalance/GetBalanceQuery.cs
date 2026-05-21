using CoreLedger.SharedKernel;
using MediatR;

namespace CoreLedger.Application.Accounts.Queries.GetBalance;

public record GetBalanceQuery(Guid AccountId, Guid RequesterId) : IRequest<Result<decimal>>;
