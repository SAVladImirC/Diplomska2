using Microsoft.AspNetCore.Diagnostics;
using VerticalSlice.Infrastructure.Common;

namespace VerticalSlice.Web.Common;

public class NotFoundExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not NotFoundException) return false;

        await Results.Problem(statusCode: StatusCodes.Status404NotFound, detail: exception.Message)
            .ExecuteAsync(httpContext);
        return true;
    }
}
