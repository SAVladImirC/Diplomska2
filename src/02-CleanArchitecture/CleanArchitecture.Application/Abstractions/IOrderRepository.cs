namespace CleanArchitecture.Application.Abstractions;

public interface IOrderRepository : IGetOrders, IAddOrder, IUpdateOrder, ISoftDeleteOrder;
