using NTier.Data.Entities;
using NTier.Data.Repositories;
using NTier.Services.Dtos;

namespace NTier.Services;

/// <summary>
/// Implements every member of <see cref="IOrderService"/>. Order validation, discount
/// calculation, tax calculation, DTO mapping and notification all live here together,
/// which is exactly the "thick service" shape the N-Tier layer tends to produce.
/// </summary>
public class OrderService(
    IRepository<Order> orders,
    IRepository<Product> products,
    IRepository<Customer> customers) : IOrderService
{
    private const decimal DiscountThreshold = 100m;
    private const decimal DiscountRate = 0.10m;
    private const decimal TaxRate = 0.18m;

    public async Task<List<OrderDto>> GetOrders()
    {
        var all = await orders.GetAllAsync();
        return all.Select(ToDto).ToList();
    }

    public async Task<OrderDto> CreateOrder(CreateOrderRequest request)
    {
        await ValidateCustomerExists(request.CustomerId);

        var order = new Order
        {
            CustomerId = request.CustomerId,
            Status = OrderStatus.Pending
        };

        foreach (var line in request.Items)
        {
            var product = await products.GetByIdAsync(line.ProductId)
                ?? throw new InvalidOperationException($"Product {line.ProductId} does not exist.");

            order.Items.Add(new OrderItem
            {
                ProductId = product.Id,
                Quantity = line.Quantity,
                UnitPrice = product.UnitPrice
            });
        }

        await orders.AddAsync(order);
        return ToDto(order);
    }

    public async Task<OrderDto> UpdateOrder(int orderId, CreateOrderRequest request)
    {
        var order = await orders.GetByIdAsync(orderId)
            ?? throw new InvalidOperationException($"Order {orderId} does not exist.");

        await ValidateCustomerExists(request.CustomerId);

        order.CustomerId = request.CustomerId;
        order.Items.Clear();
        foreach (var line in request.Items)
        {
            var product = await products.GetByIdAsync(line.ProductId)
                ?? throw new InvalidOperationException($"Product {line.ProductId} does not exist.");

            order.Items.Add(new OrderItem
            {
                ProductId = product.Id,
                Quantity = line.Quantity,
                UnitPrice = product.UnitPrice
            });
        }

        await orders.UpdateAsync(order);
        return ToDto(order);
    }

    public async Task DeleteOrder(int orderId) => await orders.DeleteAsync(orderId);

    public async Task<decimal> CalculateTax(int orderId)
    {
        var order = await orders.GetByIdAsync(orderId)
            ?? throw new InvalidOperationException($"Order {orderId} does not exist.");

        var discounted = ApplyDiscount(order.Subtotal);
        return Math.Round(discounted * TaxRate, 2);
    }

    public async Task<byte[]> GenerateInvoice(int orderId)
    {
        var order = await orders.GetByIdAsync(orderId)
            ?? throw new InvalidOperationException($"Order {orderId} does not exist.");

        var text = $"INVOICE for Order #{order.Id}\nSubtotal: {order.Subtotal:C}\nStatus: {order.Status}";
        return System.Text.Encoding.UTF8.GetBytes(text);
    }

    public async Task SendOrderConfirmation(int orderId)
    {
        var order = await orders.GetByIdAsync(orderId)
            ?? throw new InvalidOperationException($"Order {orderId} does not exist.");

        Console.WriteLine($"[NTier] Confirmation email sent for order {order.Id}.");
    }

    private async Task ValidateCustomerExists(int customerId)
    {
        _ = await customers.GetByIdAsync(customerId)
            ?? throw new InvalidOperationException($"Customer {customerId} does not exist.");
    }

    /// <summary>The recurring business rule: orders over 100 get a 10% discount.</summary>
    private static decimal ApplyDiscount(decimal subtotal) =>
        subtotal > DiscountThreshold ? subtotal * (1 - DiscountRate) : subtotal;

    private OrderDto ToDto(Order order)
    {
        var discounted = ApplyDiscount(order.Subtotal);
        return new OrderDto
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            Status = order.Status.ToString(),
            Subtotal = order.Subtotal,
            DiscountApplied = order.Subtotal - discounted,
            Total = discounted,
            CreatedAt = order.CreatedAt
        };
    }
}
