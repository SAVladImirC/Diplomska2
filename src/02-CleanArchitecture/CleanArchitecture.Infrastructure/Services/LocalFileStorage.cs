using CleanArchitecture.Application.Abstractions;

namespace CleanArchitecture.Infrastructure.Services;

public class LocalFileStorage(string rootDirectory) : IFileStorage
{
    public async Task<string> SaveAsync(string fileName, byte[] content)
    {
        Directory.CreateDirectory(rootDirectory);
        var path = Path.Combine(rootDirectory, fileName);
        await File.WriteAllBytesAsync(path, content);
        return path;
    }
}
