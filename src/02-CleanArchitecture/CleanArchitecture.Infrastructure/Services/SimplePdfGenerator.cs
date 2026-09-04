using System.Text;
using CleanArchitecture.Application.Abstractions;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Infrastructure.Services;

public class SimplePdfGenerator : IPdfGenerator
{
    public Task<byte[]> GenerateInvoiceAsync(Order order)
    {
        var text = $"INVOICE for Order #{order.Id}\nSubtotal: {order.Subtotal}\nStatus: {order.Status}";
        return Task.FromResult(Encoding.UTF8.GetBytes(text));
    }
}
