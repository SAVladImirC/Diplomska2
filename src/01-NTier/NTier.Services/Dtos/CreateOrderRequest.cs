using System.ComponentModel.DataAnnotations;

namespace NTier.Services.Dtos;

public class CreateOrderRequest
{
    [Range(1, int.MaxValue)]
    public int CustomerId { get; set; }

    [MinLength(1, ErrorMessage = "An order must contain at least one item.")]
    public List<CreateOrderItemRequest> Items { get; set; } = [];
}

public class CreateOrderItemRequest
{
    [Range(1, int.MaxValue)]
    public int ProductId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Order item quantity must be positive.")]
    public int Quantity { get; set; }
}
