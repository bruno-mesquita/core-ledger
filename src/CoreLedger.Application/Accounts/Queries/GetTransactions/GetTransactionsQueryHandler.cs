using CoreLedger.Application.Accounts.DTOs;
using CoreLedger.Domain.Accounts;
using CoreLedger.Domain.Transactions;
using CoreLedger.SharedKernel;
using MediatR;

namespace CoreLedger.Application.Accounts.Queries.GetTransactions;

public class GetTransactionsQueryHandler(
    IAccountRepository accountRepository,
    ITransactionRepository transactionRepository)
    : IRequestHandler<GetTransactionsQuery, Result<IEnumerable<TransactionResponse>>>
{
    public async Task<Result<IEnumerable<TransactionResponse>>> Handle(
        GetTransactionsQuery request, CancellationToken ct)
    {
        var account = await accountRepository.GetByIdAsync(request.AccountId, ct);
        if (account is null || account.OwnerId != request.RequesterId)
            return Result.Failure<IEnumerable<TransactionResponse>>("Account not found.");

        var transactions = await transactionRepository.GetByAccountIdAsync(
            request.AccountId, request.Page, request.PageSize, ct);

        return Result.Success(transactions.Select(TransactionResponse.FromEntity));
    }
}
