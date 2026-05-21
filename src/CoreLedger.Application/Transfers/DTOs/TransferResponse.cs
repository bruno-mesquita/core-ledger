namespace CoreLedger.Application.Transfers.DTOs;

public record TransferResponse(
    Guid TransactionId,
    Guid SourceAccountId,
    Guid DestinationAccountId,
    decimal Amount,
    string Description);
