using Microsoft.EntityFrameworkCore;
using NTier.Data;
using NTier.Data.Entities;
using NTier.Data.Repositories;
using NTier.Services;
using NTier.Services.Dtos;
using Xunit;

namespace NTier.Tests;

public class OrderServiceDiscountTests
{
    private static async Task<(AppDbContext Context, Customer Customer, Product Product)> SeedAsync()
    {
        var context = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

        var customer = new Customer { Name = "Тест Клиент", Email = "test@example.com" };
        var product = new Product { Name = "Widget", UnitPrice = 60m };
        context.Customers.Add(customer);
        context.Products.Add(product);
        await context.SaveChangesAsync();

        return (context, customer, product);
    }

    private static OrderService CreateService(AppDbContext context) =>
        new(new OrderRepository(context), new Repository<Product>(context), new Repository<Customer>(context));

    [Fact]
    public async Task CreateOrder_OverDiscountThreshold_Applies10PercentDiscount()
    {
        var (context, customer, product) = await SeedAsync();
        var service = CreateService(context);

        var order = await service.CreateOrder(new CreateOrderRequest
        {
            CustomerId = customer.Id,
            Items = [new CreateOrderItemRequest { ProductId = product.Id, Quantity = 2 }]
        });

        Assert.Equal(120m, order.Subtotal);
        Assert.Equal(12m, order.DiscountApplied);
        Assert.Equal(108m, order.Total);
    }

    [Fact]
    public async Task CreateOrder_AtOrBelowDiscountThreshold_AppliesNoDiscount()
    {
        var (context, customer, product) = await SeedAsync();
        var service = CreateService(context);

        var order = await service.CreateOrder(new CreateOrderRequest
        {
            CustomerId = customer.Id,
            Items = [new CreateOrderItemRequest { ProductId = product.Id, Quantity = 1 }]
        });

        Assert.Equal(60m, order.Subtotal);
        Assert.Equal(0m, order.DiscountApplied);
        Assert.Equal(60m, order.Total);
    }
}
