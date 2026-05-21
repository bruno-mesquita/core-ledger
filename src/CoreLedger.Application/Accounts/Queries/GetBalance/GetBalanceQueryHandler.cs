using CoreLedger.Domain.Accounts;
using CoreLedger.SharedKernel;
using MediatR;

namespace CoreLedger.Application.Accounts.Queries.GetBalance;

public class GetBalanceQueryHandler(IAccountRepository accountRepository)
    : IRequestHandler<GetBalanceQuery, Result<decimal>>
{
    public async Task<Result<decimal>> Handle(GetBalanceQuery request, CancellationToken ct)
    {
        var account = await accountRepository.GetByIdAsync(request.AccountId, ct);
        if (account is null || account.OwnerId != request.RequesterId)
            return Result.Failure<decimal>("Account not found.");

        return Result.Success(account.Balance);
    }
}
