using CleanArchitecture.Application.Abstractions;

namespace CleanArchitecture.Application.UseCases.Orders;

public class GenerateInvoiceUseCase(IGetOrders orderReader, IPdfGenerator pdfGenerator, IFileStorage fileStorage)
    : IGenerateInvoiceUseCase
{
    public async Task<byte[]?> ExecuteAsync(int orderId)
    {
        var order = await orderReader.GetByIdAsync(orderId);
        if (order is null) return null;

        var invoice = await pdfGenerator.GenerateInvoiceAsync(order);
        await fileStorage.SaveAsync($"invoice-{order.Id}.txt", invoice);
        return invoice;
    }
}
