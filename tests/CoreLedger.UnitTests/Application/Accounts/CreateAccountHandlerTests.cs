using CoreLedger.Application.Accounts.Commands.CreateAccount;
using CoreLedger.Application.Common.Interfaces;
using CoreLedger.Domain.Accounts;

namespace CoreLedger.UnitTests.Application.Accounts;

public class CreateAccountHandlerTests
{
    private readonly IAccountRepository _repo = Substitute.For<IAccountRepository>();
    private readonly IUnitOfWork _uow = Substitute.For<IUnitOfWork>();

    [Fact]
    public async Task Handle_ValidOwnerId_ReturnsSuccessWithAccount()
    {
        var ownerId = Guid.NewGuid();
        var handler = new CreateAccountCommandHandler(_repo, _uow);

        var result = await handler.Handle(new CreateAccountCommand(ownerId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Status.Should().Be("Active");
        result.Value.Balance.Should().Be(0m);
        await _repo.Received(1).AddAsync(Arg.Is<Account>(a => a.OwnerId == ownerId), Arg.Any<CancellationToken>());
    }
}
