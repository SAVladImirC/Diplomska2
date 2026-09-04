namespace CleanArchitecture.Application.UseCases.Orders;

public interface ISendOrderConfirmationUseCase
{
    Task ExecuteAsync(int orderId);
}
