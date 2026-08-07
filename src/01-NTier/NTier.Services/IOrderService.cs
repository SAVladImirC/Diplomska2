using NTier.Services.Dtos;

namespace NTier.Services;

/// <summary>
/// The single business-logic interface for everything order-related: reads, writes,
/// tax, invoicing and notifications all live behind one contract. Any consumer that
/// only needs one of these seven members is still forced to depend on all of them.
/// </summary>
public interface IOrderService
{
    Task<List<OrderDto>> GetOrders();
    Task<OrderDto> CreateOrder(CreateOrderRequest request);
    Task<OrderDto> UpdateOrder(int orderId, CreateOrderRequest request);
    Task DeleteOrder(int orderId);
    Task<decimal> CalculateTax(int orderId);
    Task<byte[]> GenerateInvoice(int orderId);
    Task SendOrderConfirmation(int orderId);
}
