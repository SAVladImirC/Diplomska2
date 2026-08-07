using NTier.Services;
using NTier.Services.Dtos;

namespace NTier.Tests.Fakes;

/// <summary>
/// A hand-written test double for <see cref="IOrderService"/>. The point of this class
/// is what it has to contain: to test a controller action that only calls
/// <see cref="GetOrders"/>, every other member of the fat interface still has to be
/// implemented here, including invoicing and email notifications the test never
/// touches. That forced breadth is the ISP violation made concrete.
/// </summary>
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
