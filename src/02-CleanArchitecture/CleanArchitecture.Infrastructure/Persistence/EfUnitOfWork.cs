using CleanArchitecture.Application.Abstractions;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Infrastructure.Persistence;

public class EfUnitOfWork(AppDbContext context, IDomainEventDispatcher dispatcher) : IUnitOfWork
{
    public async Task SaveChangesAsync()
    {
        var aggregates = context.ChangeTracker.Entries<Order>()
            .Select(e => e.Entity)
            .Where(o => o.DomainEvents.Count > 0)
            .ToList();
        var events = aggregates.SelectMany(o => o.DomainEvents).ToList();

        await context.SaveChangesAsync();

        await dispatcher.DispatchAsync(events);
        aggregates.ForEach(o => o.ClearDomainEvents());
    }
}
