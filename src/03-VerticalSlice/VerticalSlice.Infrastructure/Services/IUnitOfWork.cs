namespace VerticalSlice.Infrastructure.Services;

public interface IUnitOfWork
{
    Task SaveChangesAsync();
}
