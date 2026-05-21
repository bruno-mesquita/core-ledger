using CoreLedger.Application.Accounts.Commands.CloseAccount;
using CoreLedger.Application.Common.Interfaces;
using CoreLedger.Domain.Accounts;

namespace CoreLedger.UnitTests.Application.Accounts;

public class CloseAccountHandlerTests
{
    private readonly IAccountRepository _repo = Substitute.For<IAccountRepository>();
    private readonly IUnitOfWork _uow = Substitute.For<IUnitOfWork>();

    private CloseAccountCommandHandler CreateHandler() => new(_repo, _uow);

    [Fact]
    public async Task Handle_OwnAccount_ZeroBalance_ReturnsSuccess()
    {
        var ownerId = Guid.NewGuid();
        var account = Account.Create(ownerId);
        _repo.GetByIdAsync(account.Id).Returns(account);

        var result = await CreateHandler().Handle(
            new CloseAccountCommand(account.Id, ownerId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_NotOwner_ReturnsFailure()
    {
        var account = Account.Create(Guid.NewGuid());
        _repo.GetByIdAsync(account.Id).Returns(account);

        var result = await CreateHandler().Handle(
            new CloseAccountCommand(account.Id, Guid.NewGuid()), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_AccountNotFound_ReturnsFailure()
    {
        _repo.GetByIdAsync(Arg.Any<Guid>()).Returns((Account?)null);

        var result = await CreateHandler().Handle(
            new CloseAccountCommand(Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
    }
}
