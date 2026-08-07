namespace CleanArchitecture.Application.Abstractions;

public interface IUnitOfWork
{
    Task SaveChangesAsync();
}
