namespace VerticalSlice.Infrastructure.Services;

/// <summary>
/// The one dependency shared across several vertical slices. Changing this
/// interface's signature is the main way a change ripples across otherwise
/// isolated features (see CreateOrder and DeleteOrder, both of which use it).
/// </summary>
public class ConsoleEmailService : IEmailService
{
    public Task SendEmailAsync(string to, string subject, string body)
    {
        Console.WriteLine($"[VerticalSlice] Email to {to}: {subject}\n{body}");
        return Task.CompletedTask;
    }
}
