using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Abstractions;

public interface IPdfGenerator
{
    Task<byte[]> GenerateInvoiceAsync(Order order);
}
