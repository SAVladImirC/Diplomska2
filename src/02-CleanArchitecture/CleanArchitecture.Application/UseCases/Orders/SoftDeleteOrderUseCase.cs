using CleanArchitecture.Application.Abstractions;

namespace CleanArchitecture.Application.UseCases.Orders;

public class SoftDeleteOrderUseCase(ISoftDeleteOrder orderDeleter, IUnitOfWork unitOfWork) : ISoftDeleteOrderUseCase
{
    public async Task ExecuteAsync(int orderId)
    {
        await orderDeleter.SoftDeleteAsync(orderId);
        await unitOfWork.SaveChangesAsync();
    }
}
