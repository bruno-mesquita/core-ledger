using CoreLedger.Domain.Transactions;

namespace CoreLedger.UnitTests.Domain;

public class TransactionTests
{
    [Fact]
    public void CreateDebit_ReturnsDebitTransaction()
    {
        var accountId = Guid.NewGuid();
        var tx = Transaction.CreateDebit(accountId, 100m, "payment", "key1", TransactionKind.TED);

        tx.AccountId.Should().Be(accountId);
        tx.Amount.Should().Be(100m);
        tx.Type.Should().Be(TransactionType.Debit);
        tx.Kind.Should().Be(TransactionKind.TED);
        tx.IdempotencyKey.Should().Be("key1");
        tx.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void CreateCredit_ReturnsCreditTransaction()
    {
        var tx = Transaction.CreateCredit(Guid.NewGuid(), 50m, "deposit", "key2", TransactionKind.Deposit);

        tx.Type.Should().Be(TransactionType.Credit);
        tx.Amount.Should().Be(50m);
    }

    [Fact]
    public void ForDebit_LedgerEntry_HasCorrectDebits()
    {
        var txId = Guid.NewGuid();
        var accountId = Guid.NewGuid();

        var entry = LedgerEntry.ForDebit(txId, accountId, 75m, 25m);

        entry.TransactionId.Should().Be(txId);
        entry.Debit.Should().Be(75m);
        entry.Credit.Should().Be(0m);
        entry.RunningBalance.Should().Be(25m);
    }

    [Fact]
    public void ForCredit_LedgerEntry_HasCorrectCredits()
    {
        var entry = LedgerEntry.ForCredit(Guid.NewGuid(), Guid.NewGuid(), 200m, 350m);

        entry.Debit.Should().Be(0m);
        entry.Credit.Should().Be(200m);
        entry.RunningBalance.Should().Be(350m);
    }
}
