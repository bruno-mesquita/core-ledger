using CoreLedger.Application.Accounts.DTOs;
using CoreLedger.SharedKernel;
using MediatR;

namespace CoreLedger.Application.Accounts.Queries.GetTransactions;

public record GetTransactionsQuery(
    Guid AccountId,
    Guid RequesterId,
    int Page = 1,
    int PageSize = 20) : IRequest<Result<IEnumerable<TransactionResponse>>>;
