using Microsoft.AspNetCore.Mvc;
using NTier.Presentation.Controllers;
using NTier.Services.Dtos;
using NTier.Tests.Fakes;
using Xunit;

namespace NTier.Tests;

/// <summary>
/// Shows the ISP cost from the controller's side: even a test that only cares about
/// GetOrders has to depend on (and stand behind) the entire fat IOrderService via
/// FakeOrderService, whose other six members are unrelated to this scenario.
/// </summary>
public class OrdersControllerIspTests
{
    [Fact]
    public async Task GetOrders_ReturnsOrdersFromService()
    {
        var fakeService = new FakeOrderService
        {
            OrdersToReturn = [new OrderDto { Id = 1, CustomerId = 1, Status = "Pending" }]
        };
        var controller = new OrdersController(fakeService);

        var result = await controller.GetOrders();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var orders = Assert.IsType<List<OrderDto>>(okResult.Value);
        Assert.Single(orders);
    }
}
