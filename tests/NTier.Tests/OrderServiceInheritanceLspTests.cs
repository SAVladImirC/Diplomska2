using Microsoft.EntityFrameworkCore;
using NTier.Data;
using NTier.Data.Entities;
using NTier.Data.Repositories;
using NTier.Services;
using NTier.Services.Dtos;
using Xunit;

namespace NTier.Tests;

public class OrderServiceInheritanceLspTests
{
    private const int UnknownCustomerId = 999;

    private static async Task<(AppDbContext Context, Product Product)> SeedAsync()
    {
        var context = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

        var product = new Product { Name = "Widget", UnitPrice = 60m };
        context.Products.Add(product);
        await context.SaveChangesAsync();

        return (context, product);
    }

    private static CreateOrderRequest RequestFor(Product product) => new()
    {
        CustomerId = UnknownCustomerId,
        Items = [new CreateOrderItemRequest { ProductId = product.Id, Quantity = 1 }]
    };

    [Fact]
    public async Task BaseService_RejectsUnknownCustomer()
    {
        var (context, product) = await SeedAsync();
        IOrderService service = new OrderService(
            new OrderRepository(context), new Repository<Product>(context), new Repository<Customer>(context));

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.CreateOrder(RequestFor(product)));
    }

    [Fact]
    public async Task DerivedService_SkipsTheCheckTheBaseReliesOn_AndPersistsAnOrphanOrder()
    {
        var (context, product) = await SeedAsync();
        IOrderService service = new ExpressOrderService(
            new OrderRepository(context), new Repository<Product>(context), new Repository<Customer>(context));

        var created = await service.CreateOrder(RequestFor(product));

        Assert.Equal(UnknownCustomerId, created.CustomerId);
        Assert.False(await context.Customers.AnyAsync(c => c.Id == created.CustomerId));
    }
}
