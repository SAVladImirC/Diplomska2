using CleanArchitecture.Application.Abstractions;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Events;
using CleanArchitecture.Domain.ValueObjects;
using CleanArchitecture.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CleanArchitecture.Tests.Infrastructure;

public class EfUnitOfWorkTests
{
    private class RecordingDispatcher : IDomainEventDispatcher
    {
        public List<IDomainEvent> Dispatched { get; } = [];

        public Task DispatchAsync(IReadOnlyList<IDomainEvent> domainEvents)
        {
            Dispatched.AddRange(domainEvents);
            return Task.CompletedTask;
        }
    }

    [Fact]
    public async Task SaveChangesAsync_PersistsThenDispatchesAndClearsDomainEvents()
    {
        await using var context = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);
        var dispatcher = new RecordingDispatcher();
        var order = Order.Create(1, [new OrderItem(1, 1, new Money(50m))]);
        context.Orders.Add(order);

        await new EfUnitOfWork(context, dispatcher).SaveChangesAsync();

        var created = Assert.IsType<OrderCreatedDomainEvent>(Assert.Single(dispatcher.Dispatched));
        Assert.NotEqual(0, created.Order.Id);
        Assert.Empty(order.DomainEvents);
    }
}
