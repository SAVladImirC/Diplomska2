namespace NTier.Services.Dtos;

public class OrderDto
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal Subtotal { get; set; }
    public decimal DiscountApplied { get; set; }
    public decimal Total { get; set; }
    public DateTime CreatedAt { get; set; }
}
