using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Abstractions;

public interface IGetOrders
{
    Task<Order?> GetByIdAsync(int id);
    Task<List<Order>> GetAllAsync();
}
