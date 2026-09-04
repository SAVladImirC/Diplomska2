using CleanArchitecture.Application.Abstractions;
using CleanArchitecture.Domain.Events;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Infrastructure.Services;

public class ServiceProviderDomainEventDispatcher(IServiceProvider services) : IDomainEventDispatcher
{
    public async Task DispatchAsync(IReadOnlyList<IDomainEvent> domainEvents)
    {
        foreach (var domainEvent in domainEvents)
        {
            var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());
            var handle = handlerType.GetMethod(nameof(IDomainEventHandler<IDomainEvent>.HandleAsync))!;

            foreach (var handler in services.GetServices(handlerType))
                await (Task)handle.Invoke(handler, [domainEvent])!;
        }
    }
}
