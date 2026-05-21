using CoreLedger.Application.Accounts.DTOs;
using CoreLedger.Domain.Accounts;
using CoreLedger.SharedKernel;
using MediatR;

namespace CoreLedger.Application.Accounts.Queries.GetAccount;

public class GetAccountQueryHandler(IAccountRepository accountRepository)
    : IRequestHandler<GetAccountQuery, Result<AccountResponse>>
{
    public async Task<Result<AccountResponse>> Handle(GetAccountQuery request, CancellationToken ct)
    {
        var account = await accountRepository.GetByIdAsync(request.AccountId, ct);
        if (account is null || account.OwnerId != request.RequesterId)
            return Result.Failure<AccountResponse>("Account not found.");

        return Result.Success(AccountResponse.FromEntity(account));
    }
}
