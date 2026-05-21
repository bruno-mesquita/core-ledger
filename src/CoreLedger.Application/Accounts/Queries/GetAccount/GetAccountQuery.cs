using CoreLedger.Application.Accounts.DTOs;
using CoreLedger.SharedKernel;
using MediatR;

namespace CoreLedger.Application.Accounts.Queries.GetAccount;

public record GetAccountQuery(Guid AccountId, Guid RequesterId) : IRequest<Result<AccountResponse>>;
