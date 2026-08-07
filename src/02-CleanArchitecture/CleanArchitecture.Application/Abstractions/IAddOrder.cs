using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Abstractions;

public interface IAddOrder
{
    Task AddAsync(Order order);
}
