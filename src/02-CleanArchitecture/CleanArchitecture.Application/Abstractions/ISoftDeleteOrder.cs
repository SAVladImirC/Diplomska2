namespace CleanArchitecture.Application.Abstractions;

public interface ISoftDeleteOrder
{
    Task SoftDeleteAsync(int orderId);
}
