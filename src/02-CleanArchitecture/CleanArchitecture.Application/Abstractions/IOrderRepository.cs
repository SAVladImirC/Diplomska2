namespace CleanArchitecture.Application.Abstractions;

/// <summary>
/// Composes the tiny role interfaces rather than exposing one broad repository
/// contract. Consumers can (and should) depend on just the one or two roles they
/// actually need instead of this composed interface.
/// </summary>
public interface IOrderRepository : IGetOrders, IAddOrder, IUpdateOrder, ISoftDeleteOrder;
