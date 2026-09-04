using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NTier.Data;
using NTier.Data.Entities;
using NTier.Services;
using NTier.Services.Dtos;

namespace NTier.Presentation.Controllers;

[ApiController]
[Route("orders")]
public class OrdersController(IOrderService orderService, AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<OrderDto>>> GetOrders() =>
        Ok(await orderService.GetOrders());

    [HttpGet("{orderId:int}")]
    public async Task<ActionResult<Order>> GetOrder(int orderId)
    {
        var order = await db.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == orderId);
        return order is null ? NotFound() : Ok(order);
    }

    [HttpPost]
    public async Task<ActionResult<OrderDto>> CreateOrder(CreateOrderRequest request) =>
        Ok(await orderService.CreateOrder(request));

    [HttpPut("{orderId:int}")]
    public async Task<ActionResult<OrderDto>> UpdateOrder(int orderId, CreateOrderRequest request) =>
        Ok(await orderService.UpdateOrder(orderId, request));

    [HttpDelete("{orderId:int}")]
    public async Task<IActionResult> DeleteOrder(int orderId)
    {
        await orderService.DeleteOrder(orderId);
        return NoContent();
    }

    [HttpGet("{orderId:int}/tax")]
    public async Task<ActionResult<decimal>> CalculateTax(int orderId) =>
        Ok(await orderService.CalculateTax(orderId));

    [HttpGet("{orderId:int}/invoice")]
    public async Task<IActionResult> GenerateInvoice(int orderId) =>
        File(await orderService.GenerateInvoice(orderId), "text/plain", $"invoice-{orderId}.txt");

    [HttpPost("{orderId:int}/send-confirmation")]
    public async Task<IActionResult> SendOrderConfirmation(int orderId)
    {
        await orderService.SendOrderConfirmation(orderId);
        return NoContent();
    }
}
