using FluentValidation;

namespace CoreLedger.Application.Transfers.Commands.CreateTransfer;

public class CreateTransferCommandValidator : AbstractValidator<CreateTransferCommand>
{
    public CreateTransferCommandValidator()
    {
        RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Amount must be positive.");
        RuleFor(x => x.IdempotencyKey).NotEmpty().MaximumLength(256);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(512);
        RuleFor(x => x).Must(x => x.SourceAccountId != x.DestinationAccountId)
            .WithMessage("Source and destination accounts must differ.");
    }
}
