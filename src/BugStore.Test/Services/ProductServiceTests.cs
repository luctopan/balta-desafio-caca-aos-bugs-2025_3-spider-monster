using BugStore.Dtos;
using BugStore.Models;
using BugStore.Repositories;
using BugStore.Services;
using Moq;

namespace BugStore.Test.Services;

public class ProductServiceTests
{
    [Fact]
    public async Task CreateAsync_ShouldReturnProductDto()
    {
        var ct = TestContext.Current.CancellationToken;

        Product? captured = null;

        var repo = new Mock<IProductRepository>(MockBehavior.Strict);
        repo.Setup(x => x.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .Callback<Product, CancellationToken>((p, _) => captured = p)
            .ReturnsAsync((Product p, CancellationToken _) => p);

        var service = new ProductService(repo.Object);

        var input = new ProductCreateDto("Headset", "Gamer", "headset", 299.99m);
        var dto = await service.CreateAsync(input, ct);

        Assert.NotNull(captured);
        Assert.NotEqual(Guid.Empty, captured!.Id);
        Assert.Equal(captured.Id, dto.Id);
        Assert.Equal("Headset", dto.Title);

        repo.VerifyAll();
    }
}