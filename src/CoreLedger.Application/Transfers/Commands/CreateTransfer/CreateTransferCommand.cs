using CoreLedger.Application.Transfers.DTOs;
using CoreLedger.SharedKernel;
using MediatR;

namespace CoreLedger.Application.Transfers.Commands.CreateTransfer;

public record CreateTransferCommand(
    Guid SourceAccountId,
    Guid DestinationAccountId,
    decimal Amount,
    string Description,
    string IdempotencyKey,
    Guid RequesterId) : IRequest<Result<TransferResponse>>;
