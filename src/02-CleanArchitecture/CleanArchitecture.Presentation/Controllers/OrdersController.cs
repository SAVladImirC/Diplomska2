using CleanArchitecture.Application.Dtos;
using CleanArchitecture.Application.UseCases.Orders;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.Presentation.Controllers;

[ApiController]
[Route("orders")]
public class OrdersController(
    IGetOrdersUseCase getOrders,
    IGetOrderByIdUseCase getOrderById,
    ICreateOrderUseCase createOrder,
    IUpdateOrderUseCase updateOrder,
    ISoftDeleteOrderUseCase deleteOrder,
    IGenerateInvoiceUseCase generateInvoice,
    ISendOrderConfirmationUseCase sendConfirmation) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<OrderDto>>> GetOrders() => Ok(await getOrders.ExecuteAsync());

    [HttpGet("{orderId:int}")]
    public async Task<ActionResult<OrderDto>> GetOrder(int orderId)
    {
        var order = await getOrderById.ExecuteAsync(orderId);
        return order is null ? NotFound() : Ok(order);
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
        await deleteOrder.ExecuteAsync(orderId);
        return NoContent();
    }

    [HttpGet("{orderId:int}/invoice")]
    public async Task<IActionResult> GenerateInvoice(int orderId)
    {
        var invoice = await generateInvoice.ExecuteAsync(orderId);
        return invoice is null ? NotFound() : File(invoice, "text/plain", $"invoice-{orderId}.txt");
    }

    [HttpPost("{orderId:int}/send-confirmation")]
    public async Task<IActionResult> SendOrderConfirmation(int orderId)
    {
        await sendConfirmation.ExecuteAsync(orderId);
        return NoContent();
    }
}
