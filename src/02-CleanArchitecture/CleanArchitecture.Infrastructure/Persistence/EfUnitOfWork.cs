using CleanArchitecture.Application.Abstractions;

namespace CleanArchitecture.Infrastructure.Persistence;

public class EfUnitOfWork(AppDbContext context) : IUnitOfWork
{
    public Task SaveChangesAsync() => context.SaveChangesAsync();
}
