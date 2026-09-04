using NTier.Services;
using NTier.Services.Dtos;

namespace NTier.Tests.Fakes;

public class FakeOrderService : IOrderService
{
    public List<OrderDto> OrdersToReturn { get; set; } = [];

    public Task<List<OrderDto>> GetOrders() => Task.FromResult(OrdersToReturn);

    public Task<OrderDto> CreateOrder(CreateOrderRequest request) =>
        throw new NotImplementedException("Not needed for this test, but still had to be written.");

    public Task<OrderDto> UpdateOrder(int orderId, CreateOrderRequest request) =>
        throw new NotImplementedException("Not needed for this test, but still had to be written.");

    public Task DeleteOrder(int orderId) =>
        throw new NotImplementedException("Not needed for this test, but still had to be written.");

    public Task<decimal> CalculateTax(int orderId) =>
        throw new NotImplementedException("Not needed for this test, but still had to be written.");

    public Task<byte[]> GenerateInvoice(int orderId) =>
        throw new NotImplementedException("Not needed for this test, but still had to be written.");

    public Task SendOrderConfirmation(int orderId) =>
        throw new NotImplementedException("Not needed for this test, but still had to be written.");
}
