using NTier.Services.Dtos;

namespace NTier.Services;

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
