using BugStore.Dtos;
using BugStore.Models;
using BugStore.Repositories;
using BugStore.Services;
using Moq;

namespace BugStore.Test.Services;

public class CustomerServiceTests
{
    [Fact]
    public async Task CreateAsync_ShouldReturnCustomerDto()
    {
        var ct = TestContext.Current.CancellationToken;

        Customer? captured = null;

        var repo = new Mock<ICustomerRepository>(MockBehavior.Strict);
        repo.Setup(x => x.AddAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()))
            .Callback<Customer, CancellationToken>((c, _) => captured = c)
            .ReturnsAsync((Customer c, CancellationToken _) => c);

        var service = new CustomerService(repo.Object);

        var input = new CustomerCreateDto("Alex", "alex@test.com", "19999999999", new DateTime(1992, 9, 19));
        var dto = await service.CreateAsync(input, ct);

        Assert.NotNull(captured);
        Assert.NotEqual(Guid.Empty, captured!.Id);
        Assert.Equal(captured.Id, dto.Id);
        Assert.Equal("Alex", dto.Name);

        repo.VerifyAll();
    }
}