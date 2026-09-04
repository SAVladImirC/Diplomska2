namespace CleanArchitecture.Application.UseCases.Orders;

public interface IGenerateInvoiceUseCase
{
    Task<byte[]?> ExecuteAsync(int orderId);
}
