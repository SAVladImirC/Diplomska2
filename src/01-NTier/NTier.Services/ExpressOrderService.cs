using NTier.Data.Entities;
using NTier.Data.Repositories;

namespace NTier.Services;

public class ExpressOrderService(
    IRepository<Order> orders,
    IRepository<Product> products,
    IRepository<Customer> customers) : OrderService(orders, products, customers)
{
    protected override Task ValidateCustomerExists(int customerId) => Task.CompletedTask;
}
