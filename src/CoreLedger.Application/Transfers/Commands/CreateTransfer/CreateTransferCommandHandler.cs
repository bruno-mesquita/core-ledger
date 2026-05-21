using CoreLedger.Application.Common.Interfaces;
using CoreLedger.Application.Transfers.DTOs;
using CoreLedger.Domain.Accounts;
using CoreLedger.Domain.Transactions;
using CoreLedger.SharedKernel;
using MediatR;

namespace CoreLedger.Application.Transfers.Commands.CreateTransfer;

public class CreateTransferCommandHandler(
    IAccountRepository accountRepository,
    ITransactionRepository transactionRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateTransferCommand, Result<TransferResponse>>
{
    public async Task<Result<TransferResponse>> Handle(CreateTransferCommand request, CancellationToken ct)
    {
        // Idempotency check
        var existing = await transactionRepository.GetByIdempotencyKeyAsync(request.IdempotencyKey, ct);
        if (existing is not null)
            return Result.Success(new TransferResponse(
                existing.Id,
                request.SourceAccountId,
                request.DestinationAccountId,
                existing.Amount,
                existing.Description));

        var source = await accountRepository.GetByIdAsync(request.SourceAccountId, ct);
        if (source is null || source.OwnerId != request.RequesterId)
            return Result.Failure<TransferResponse>("Source account not found.");

        var destination = await accountRepository.GetByIdAsync(request.DestinationAccountId, ct);
        if (destination is null)
            return Result.Failure<TransferResponse>("Destination account not found.");

        var debitResult = source.Debit(request.Amount);
        if (debitResult.IsFailure)
            return Result.Failure<TransferResponse>(debitResult.Error!);

        var creditResult = destination.Credit(request.Amount);
        if (creditResult.IsFailure)
            return Result.Failure<TransferResponse>(creditResult.Error!);

        var debitTx = Transaction.CreateDebit(
            source.Id, request.Amount, request.Description,
            request.IdempotencyKey, TransactionKind.TED, destination.Id);

        var creditTx = Transaction.CreateCredit(
            destination.Id, request.Amount, request.Description,
            request.IdempotencyKey + "-credit", TransactionKind.TED, source.Id);

        var debitEntry = LedgerEntry.ForDebit(debitTx.Id, source.Id, request.Amount, source.Balance);
        var creditEntry = LedgerEntry.ForCredit(creditTx.Id, destination.Id, request.Amount, destination.Balance);

        await accountRepository.UpdateAsync(source, ct);
        await accountRepository.UpdateAsync(destination, ct);
        await transactionRepository.AddRangeAsync([debitTx, creditTx], ct);
        await transactionRepository.AddLedgerEntriesAsync([debitEntry, creditEntry], ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(new TransferResponse(
            debitTx.Id,
            source.Id,
            destination.Id,
            request.Amount,
            request.Description));
    }
}
