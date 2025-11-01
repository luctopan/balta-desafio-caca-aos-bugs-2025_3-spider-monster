using BugStore.Dtos;
using BugStore.Models;
using BugStore.Repositories;
using BugStore.Services;
using Moq;

namespace BugStore.Test.Services;

public class OrderServiceTests
{
    [Fact]
    public async Task CreateAsync_ShouldCalculateTotalsAndReturnDto()
    {
        var customerId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        var product = new Product
        {
            Id = productId,
            Title = "Keyboard",
            Price = 200m
        };

        var productRepo = new Mock<IProductRepository>();
        productRepo.Setup(x => x.GetAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        Order? capturedOrder = null;

        var orderRepo = new Mock<IOrderRepository>();
        orderRepo.Setup(x => x.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
            .Callback<Order, CancellationToken>((o, _) => capturedOrder = o)
            .ReturnsAsync((Order o, CancellationToken _) => o);

        var service = new OrderService(orderRepo.Object, productRepo.Object);

        var dto = await service.CreateAsync(customerId, new()
        {
            new OrderLineCreateDto(productId, 2)
        }, TestContext.Current.CancellationToken);

        Assert.NotNull(capturedOrder);
        Assert.Single(capturedOrder!.Lines);
        Assert.Equal(400m, capturedOrder.Lines[0].Total);
        Assert.Equal(400m, dto.Lines[0].Total);
        Assert.Equal(customerId, dto.CustomerId);

        orderRepo.Verify(x => x.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Once);
        productRepo.Verify(x => x.GetAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
    }
}