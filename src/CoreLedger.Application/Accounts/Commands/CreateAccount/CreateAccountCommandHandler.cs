using CoreLedger.Application.Accounts.DTOs;
using CoreLedger.Application.Common.Interfaces;
using CoreLedger.Domain.Accounts;
using CoreLedger.SharedKernel;
using MediatR;

namespace CoreLedger.Application.Accounts.Commands.CreateAccount;

public class CreateAccountCommandHandler(
    IAccountRepository accountRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateAccountCommand, Result<AccountResponse>>
{
    public async Task<Result<AccountResponse>> Handle(CreateAccountCommand request, CancellationToken ct)
    {
        var account = Account.Create(request.OwnerId);
        await accountRepository.AddAsync(account, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success(AccountResponse.FromEntity(account));
    }
}
