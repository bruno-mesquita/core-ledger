using CoreLedger.Application.Common.Interfaces;
using CoreLedger.Domain.Accounts;
using CoreLedger.SharedKernel;
using MediatR;

namespace CoreLedger.Application.Accounts.Commands.CloseAccount;

public class CloseAccountCommandHandler(
    IAccountRepository accountRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CloseAccountCommand, Result>
{
    public async Task<Result> Handle(CloseAccountCommand request, CancellationToken ct)
    {
        var account = await accountRepository.GetByIdAsync(request.AccountId, ct);
        if (account is null || account.OwnerId != request.RequesterId)
            return Result.Failure("Account not found.");

        var closeResult = account.Close();
        if (closeResult.IsFailure)
            return closeResult;

        await accountRepository.UpdateAsync(account, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
