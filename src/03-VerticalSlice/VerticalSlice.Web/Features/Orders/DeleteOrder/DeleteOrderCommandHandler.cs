using MediatR;
using Microsoft.EntityFrameworkCore;
using VerticalSlice.Infrastructure.Persistence;
using VerticalSlice.Infrastructure.Services;

namespace VerticalSlice.Web.Features.Orders.DeleteOrder;

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
