using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Abstractions;

public interface ICustomerLookup
{
    Task<Customer?> GetByIdAsync(int id);
}
