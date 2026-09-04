namespace VerticalSlice.Infrastructure.Services;

public class ConsoleEmailService : IEmailService
{
    public Task SendEmailAsync(string to, string subject, string body)
    {
        Console.WriteLine($"[VerticalSlice] Email to {to}: {subject}\n{body}");
        return Task.CompletedTask;
    }
}
