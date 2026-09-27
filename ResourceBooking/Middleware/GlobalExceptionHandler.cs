using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ResourceBooking.Exceptions;

namespace ResourceBooking.Middleware
{
    /// <summary>
    /// Catches unhandled exceptions, logs them, and returns a safe RFC 7807
    /// ProblemDetails response instead of leaking exception details to clients.
    /// </summary>
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;
        private readonly IProblemDetailsService _problemDetailsService;

        public GlobalExceptionHandler(
            ILogger<GlobalExceptionHandler> logger,
            IProblemDetailsService problemDetailsService
        )
        {
            _logger = logger;
            _problemDetailsService = problemDetailsService;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken
        )
        {
            _logger.LogError(
                exception,
                "Unhandled exception while processing {Method} {Path}",
                httpContext.Request.Method,
                httpContext.Request.Path
            );

            var (statusCode, title, detail) = exception switch
            {
                ResourceAlreadyBookedException => (
                    StatusCodes.Status409Conflict,
                    "Resource already booked",
                    exception.Message
                ),
                _ => (
                    StatusCodes.Status500InternalServerError,
                    "An unexpected error occurred.",
                    (string?)null
                ),
            };

            httpContext.Response.StatusCode = statusCode;

            return await _problemDetailsService.TryWriteAsync(
                new ProblemDetailsContext
                {
                    HttpContext = httpContext,
                    ProblemDetails = new ProblemDetails
                    {
                        Status = statusCode,
                        Title = title,
                        Detail = detail,
                    },
                }
            );
        }
    }
}
