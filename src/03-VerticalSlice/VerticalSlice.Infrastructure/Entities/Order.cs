namespace VerticalSlice.Infrastructure.Entities;

public class Order
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; }

    public List<OrderItem> Items { get; set; } = [];

    public decimal Subtotal => Items.Sum(i => i.LineTotal);
}
