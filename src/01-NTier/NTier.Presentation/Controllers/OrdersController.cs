using Microsoft.AspNetCore.Mvc;
using NTier.Services;
using NTier.Services.Dtos;

namespace NTier.Presentation.Controllers;

/// <summary>
/// Depends on the entire <see cref="IOrderService"/>, even though a given action
/// (e.g. listing orders) only ever calls one of its seven members.
/// </summary>
[ApiController]
[Route("orders")]
public class OrdersController(IOrderService orderService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<OrderDto>>> GetOrders() =>
        Ok(await orderService.GetOrders());

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
