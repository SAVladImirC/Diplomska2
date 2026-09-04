namespace CleanArchitecture.Application.Abstractions;

public interface IFileStorage
{
    Task<string> SaveAsync(string fileName, byte[] content);
}
