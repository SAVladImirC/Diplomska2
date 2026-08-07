using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Abstractions;

public interface IUpdateOrder
{
    Task UpdateAsync(Order order);
}
