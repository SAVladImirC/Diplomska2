using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Abstractions;

public interface IProductLookup
{
    Task<Product?> GetByIdAsync(int id);
}
