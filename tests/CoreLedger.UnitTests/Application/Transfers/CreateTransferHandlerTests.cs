using CoreLedger.Application.Common.Interfaces;
using CoreLedger.Application.Transfers.Commands.CreateTransfer;
using CoreLedger.Domain.Accounts;
using CoreLedger.Domain.Transactions;

namespace CoreLedger.UnitTests.Application.Transfers;

public class CreateTransferHandlerTests
{
    private readonly IAccountRepository _accountRepo = Substitute.For<IAccountRepository>();
    private readonly ITransactionRepository _txRepo = Substitute.For<ITransactionRepository>();
    private readonly IUnitOfWork _uow = Substitute.For<IUnitOfWork>();

    private CreateTransferCommandHandler CreateHandler() =>
        new(_accountRepo, _txRepo, _uow);

    private Account FundedAccount(Guid ownerId, decimal balance = 500m)
    {
        var account = Account.Create(ownerId);
        account.Credit(balance);
        return account;
    }

    [Fact]
    public async Task Handle_ValidTransfer_DebitSourceCreditDest()
    {
        var ownerId = Guid.NewGuid();
        var source = FundedAccount(ownerId);
        var dest = Account.Create(Guid.NewGuid());

        _txRepo.GetByIdempotencyKeyAsync("key1").Returns((Transaction?)null);
        _accountRepo.GetByIdAsync(source.Id).Returns(source);
        _accountRepo.GetByIdAsync(dest.Id).Returns(dest);

        var command = new CreateTransferCommand(
            source.Id, dest.Id, 100m, "payment", "key1", ownerId);

        var result = await CreateHandler().Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        source.Balance.Should().Be(400m);
        dest.Balance.Should().Be(100m);
    }

    [Fact]
    public async Task Handle_InsufficientFunds_ReturnsFailure()
    {
        var ownerId = Guid.NewGuid();
        var source = Account.Create(ownerId); // balance = 0
        var dest = Account.Create(Guid.NewGuid());

        _txRepo.GetByIdempotencyKeyAsync("key2").Returns((Transaction?)null);
        _accountRepo.GetByIdAsync(source.Id).Returns(source);
        _accountRepo.GetByIdAsync(dest.Id).Returns(dest);

        var result = await CreateHandler().Handle(
            new CreateTransferCommand(source.Id, dest.Id, 100m, "pay", "key2", ownerId),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Insufficient");
    }

    [Fact]
    public async Task Handle_IdempotentKey_ReturnsExistingTransaction()
    {
        var existing = Transaction.CreateDebit(
            Guid.NewGuid(), 100m, "pay", "key3", TransactionKind.TED);
        _txRepo.GetByIdempotencyKeyAsync("key3").Returns(existing);

        var result = await CreateHandler().Handle(
            new CreateTransferCommand(
                Guid.NewGuid(), Guid.NewGuid(), 100m, "pay", "key3", Guid.NewGuid()),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.TransactionId.Should().Be(existing.Id);
        await _accountRepo.DidNotReceive().GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_SourceNotOwnedByRequester_ReturnsFailure()
    {
        var source = Account.Create(Guid.NewGuid()); // different owner
        source.Credit(500m);
        var dest = Account.Create(Guid.NewGuid());

        _txRepo.GetByIdempotencyKeyAsync("key4").Returns((Transaction?)null);
        _accountRepo.GetByIdAsync(source.Id).Returns(source);
        _accountRepo.GetByIdAsync(dest.Id).Returns(dest);

        var result = await CreateHandler().Handle(
            new CreateTransferCommand(source.Id, dest.Id, 100m, "pay", "key4", Guid.NewGuid()),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("not found");
    }
}
