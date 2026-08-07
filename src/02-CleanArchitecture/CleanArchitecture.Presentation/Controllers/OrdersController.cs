using CleanArchitecture.Application.Abstractions;
using CleanArchitecture.Application.Dtos;
using CleanArchitecture.Application.UseCases.Orders;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.Presentation.Controllers;

/// <summary>
/// Depends on several small, specific abstractions instead of one fat service --
/// contrast with NTier.Presentation.Controllers.OrdersController, which injects the
/// entire IOrderService for every action.
/// </summary>
[ApiController]
[Route("orders")]
public class OrdersController(
    IGetOrders orderReader,
    ICreateOrderUseCase createOrder,
    IUpdateOrderUseCase updateOrder,
    ISoftDeleteOrder deleteOrder,
    ICalculateOrderTotalUseCase totalCalculator,
    IPdfGenerator pdfGenerator,
    IEmailService emailService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<OrderDto>>> GetOrders()
    {
        var orders = await orderReader.GetAllAsync();
        return Ok(orders.Select(o => OrderMapper.ToDto(o, totalCalculator)).ToList());
    }

    [HttpGet("{orderId:int}")]
    public async Task<ActionResult<OrderDto>> GetOrder(int orderId)
    {
        var order = await orderReader.GetByIdAsync(orderId);
        return order is null ? NotFound() : Ok(OrderMapper.ToDto(order, totalCalculator));
    }

    [HttpPost]
    public async Task<ActionResult<OrderDto>> CreateOrder(CreateOrderRequest request) =>
        Ok(await createOrder.ExecuteAsync(request));

    [HttpPut("{orderId:int}")]
    public async Task<ActionResult<OrderDto>> UpdateOrder(int orderId, CreateOrderRequest request) =>
        Ok(await updateOrder.ExecuteAsync(orderId, request));

    [HttpDelete("{orderId:int}")]
    public async Task<IActionResult> DeleteOrder(int orderId)
    {
        await deleteOrder.SoftDeleteAsync(orderId);
        return NoContent();
    }

    [HttpGet("{orderId:int}/invoice")]
    public async Task<IActionResult> GenerateInvoice(int orderId)
    {
        var order = await orderReader.GetByIdAsync(orderId);
        if (order is null) return NotFound();

        var bytes = await pdfGenerator.GenerateInvoiceAsync(order);
        return File(bytes, "text/plain", $"invoice-{orderId}.txt");
    }

    [HttpPost("{orderId:int}/send-confirmation")]
    public async Task<IActionResult> SendOrderConfirmation(int orderId)
    {
        var order = await orderReader.GetByIdAsync(orderId);
        if (order is null) return NotFound();

        await emailService.SendEmailAsync(
            "customer@example.com", "Order Confirmation", $"Your order #{order.Id} has been received.");
        return NoContent();
    }
}
