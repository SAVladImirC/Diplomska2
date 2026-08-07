using MediatR;
using Microsoft.EntityFrameworkCore;
using VerticalSlice.Infrastructure.Persistence;
using VerticalSlice.Infrastructure.Services;

namespace VerticalSlice.Web.Features.Orders.DeleteOrder;

/// <summary>
/// Soft-deletes directly against AppDbContext -- there is no shared generic
/// repository here for a status check to surprise, unlike N-Tier's
/// IRepository&lt;Order&gt;.DeleteAsync. IEmailService is the one dependency this
/// slice shares with CreateOrder; changing its signature is the one way a change
/// here would ripple into another, otherwise unrelated, slice.
/// </summary>
public class DeleteOrderCommandHandler(AppDbContext db, IEmailService emailService) : IRequestHandler<DeleteOrderCommand>
{
    public async Task Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await db.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

        if (order is null) return;

        order.IsDeleted = true;
        await db.SaveChangesAsync(cancellationToken);

        var customer = await db.Customers.FindAsync([order.CustomerId], cancellationToken);
        if (customer is not null)
        {
            await emailService.SendEmailAsync(
                customer.Email, "Order Cancelled", $"Your order #{order.Id} has been cancelled.");
        }
    }
}
