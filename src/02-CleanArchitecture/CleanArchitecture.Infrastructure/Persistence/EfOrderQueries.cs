using CleanArchitecture.Application.Abstractions;
using CleanArchitecture.Application.ReadModels;
using CleanArchitecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Infrastructure.Persistence;

public class EfOrderQueries(AppDbContext context) : IOrderQueries
{
    public async Task<List<OrderReadModel>> GetAllAsync()
    {
        var rows = await Project(context.Orders.Where(o => !o.IsDeleted)).ToListAsync();
        return rows.Select(ToReadModel).ToList();
    }

    public async Task<OrderReadModel?> GetByIdAsync(int orderId)
    {
        var row = await Project(context.Orders.Where(o => o.Id == orderId && !o.IsDeleted)).FirstOrDefaultAsync();
        return row is null ? null : ToReadModel(row);
    }

    private static IQueryable<OrderRow> Project(IQueryable<Order> orders) =>
        orders.AsNoTracking().Select(o => new OrderRow(
            o.Id,
            o.CustomerId,
            o.Status,
            o.CreatedAt,
            o.Items.Select(i => new LineRow(i.Quantity, i.UnitPrice.Amount)).ToList()));

    private static OrderReadModel ToReadModel(OrderRow row) =>
        new(row.Id, row.CustomerId, row.Status, row.Lines.Sum(l => l.Quantity * l.Amount), row.CreatedAt);

    private record OrderRow(int Id, int CustomerId, Domain.Enums.OrderStatus Status, DateTime CreatedAt, List<LineRow> Lines);

    private record LineRow(int Quantity, decimal Amount);
}
