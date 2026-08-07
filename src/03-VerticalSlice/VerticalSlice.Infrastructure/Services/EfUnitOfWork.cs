using VerticalSlice.Infrastructure.Persistence;

namespace VerticalSlice.Infrastructure.Services;

public class EfUnitOfWork(AppDbContext context) : IUnitOfWork
{
    public Task SaveChangesAsync() => context.SaveChangesAsync();
}
