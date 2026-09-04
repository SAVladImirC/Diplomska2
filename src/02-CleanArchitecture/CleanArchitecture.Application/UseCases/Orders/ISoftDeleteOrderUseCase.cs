namespace CleanArchitecture.Application.UseCases.Orders;

public interface ISoftDeleteOrderUseCase
{
    Task ExecuteAsync(int orderId);
}
