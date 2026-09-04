using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NTier.Data;
using NTier.Presentation.Controllers;
using NTier.Services.Dtos;
using NTier.Tests.Fakes;
using Xunit;

namespace NTier.Tests;

public class OrdersControllerIspTests
{
    private static AppDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    [Fact]
    public async Task GetOrders_ReturnsOrdersFromService()
    {
        var fakeService = new FakeOrderService
        {
            OrdersToReturn = [new OrderDto { Id = 1, CustomerId = 1, Status = "Pending" }]
        };
        await using var context = CreateContext();
        var controller = new OrdersController(fakeService, context);

        var result = await controller.GetOrders();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var orders = Assert.IsType<List<OrderDto>>(okResult.Value);
        Assert.Single(orders);
    }

    [Fact]
    public async Task GetOrder_BypassesTheServiceLayerEntirely()
    {
        await using var context = CreateContext();
        context.Orders.Add(new NTier.Data.Entities.Order { CustomerId = 1 });
        await context.SaveChangesAsync();
        var controller = new OrdersController(new FakeOrderService(), context);

        var result = await controller.GetOrder(1);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.IsType<NTier.Data.Entities.Order>(okResult.Value);
    }
}
