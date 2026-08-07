using CleanArchitecture.Application.Abstractions;

namespace CleanArchitecture.Infrastructure.Services;

/// <summary>Logs to the console instead of calling a real SMTP server.</summary>
public class ConsoleEmailService : IEmailService
{
    public Task SendEmailAsync(string to, string subject, string body)
    {
        Console.WriteLine($"[CleanArchitecture] Email to {to}: {subject}\n{body}");
        return Task.CompletedTask;
    }
}
