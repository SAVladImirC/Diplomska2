using CleanArchitecture.Application.ReadModels;

namespace CleanArchitecture.Application.Abstractions;

public interface IOrderQueries
{
    Task<List<OrderReadModel>> GetAllAsync();
    Task<OrderReadModel?> GetByIdAsync(int orderId);
}
