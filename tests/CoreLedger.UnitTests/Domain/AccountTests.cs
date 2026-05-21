using CoreLedger.Domain.Accounts;

namespace CoreLedger.UnitTests.Domain;

public class AccountTests
{
    [Fact]
    public void Create_ReturnsActiveAccountWithZeroBalance()
    {
        var ownerId = Guid.NewGuid();
        var account = Account.Create(ownerId);

        account.OwnerId.Should().Be(ownerId);
        account.Balance.Should().Be(0);
        account.Status.Should().Be(AccountStatus.Active);
        account.AccountNumber.Should().HaveLength(10);
    }

    [Fact]
    public void Debit_SufficientFunds_DecreasesBalance()
    {
        var account = Account.Create(Guid.NewGuid());
        account.Credit(100m);

        var result = account.Debit(60m);

        result.IsSuccess.Should().BeTrue();
        account.Balance.Should().Be(40m);
    }

    [Fact]
    public void Debit_InsufficientFunds_ReturnsFailure()
    {
        var account = Account.Create(Guid.NewGuid());

        var result = account.Debit(50m);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Insufficient");
        account.Balance.Should().Be(0);
    }

    [Fact]
    public void Debit_NegativeAmount_ReturnsFailure()
    {
        var account = Account.Create(Guid.NewGuid());
        account.Credit(100m);

        var result = account.Debit(-10m);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Debit_ClosedAccount_ReturnsFailure()
    {
        var account = Account.Create(Guid.NewGuid());
        account.Close();

        var result = account.Debit(10m);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("closed");
    }

    [Fact]
    public void Credit_ValidAmount_IncreasesBalance()
    {
        var account = Account.Create(Guid.NewGuid());

        var result = account.Credit(150m);

        result.IsSuccess.Should().BeTrue();
        account.Balance.Should().Be(150m);
    }

    [Fact]
    public void Credit_NegativeAmount_ReturnsFailure()
    {
        var account = Account.Create(Guid.NewGuid());

        var result = account.Credit(-50m);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Close_ZeroBalance_ClosesAccount()
    {
        var account = Account.Create(Guid.NewGuid());

        var result = account.Close();

        result.IsSuccess.Should().BeTrue();
        account.Status.Should().Be(AccountStatus.Closed);
        account.ClosedAt.Should().NotBeNull();
    }

    [Fact]
    public void Close_NonZeroBalance_ReturnsFailure()
    {
        var account = Account.Create(Guid.NewGuid());
        account.Credit(100m);

        var result = account.Close();

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("non-zero balance");
    }

    [Fact]
    public void Close_AlreadyClosed_ReturnsFailure()
    {
        var account = Account.Create(Guid.NewGuid());
        account.Close();

        var result = account.Close();

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Debit_WithLimit_AllowsDebitBeyondBalance()
    {
        var account = Account.Create(Guid.NewGuid());
        account.SetLimit(500m);

        var result = account.Debit(300m);

        result.IsSuccess.Should().BeTrue();
        account.Balance.Should().Be(-300m);
    }
}
