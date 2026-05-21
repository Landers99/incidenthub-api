using System.Diagnostics;
using IncidentHub.Api.Extensions;

namespace IncidentHub.Api.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(
            RequestDelegate next,
            ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        await _next(context);

        stopwatch.Stop();

        var correlationId = context.GetCorrelationId();
        var userId = context.User?.FindFirst("userId")?.Value ?? "anonymous";

        _logger.LogInformation(
                "HTTP {Method} {Path} responded {StatusCode} in {ElapsedMs}ms UserId={UserId} CorrelationId={CorrelationId}",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds,
                userId,
                correlationId);

        if (stopwatch.ElapsedMilliseconds > 500)
        {
            _logger.LogWarning(
                "Slow request detected. Method={Method} Path={Path} ElapsedMs={ElapsedMs} UserId={UserId} CorrelationId={CorrelationId}",
                context.Request.Method,
                context.Request.Path,
                stopwatch.ElapsedMilliseconds,
                userId,
                correlationId);
        }
    }
}
