using BugStore.Data;
using BugStore.Models;
using BugStore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BugStore.Test.Repositories;

public class ProductRepositoryTests
{
    private static AppDbContext GetContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task AddAndGetAsync_ShouldPersistProduct()
    {
        var ct = TestContext.Current.CancellationToken;

        await using var ctx = GetContext();
        var repo = new ProductRepository(ctx);

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Title = "Monitor",
            Description = "29 inch",
            Slug = "monitor",
            Price = 1200m
        };

        await repo.AddAsync(product, ct);
        var saved = await repo.GetAsync(product.Id, ct);

        Assert.NotNull(saved);
        Assert.Equal(product.Id, saved!.Id);
        Assert.Equal("Monitor", saved.Title);
    }
}