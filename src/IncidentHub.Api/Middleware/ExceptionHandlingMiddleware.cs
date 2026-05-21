using System.Net;
using IncidentHub.Api.Common;
using IncidentHub.Api.Extensions;

namespace IncidentHub.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger,
            IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var correlationId = context.GetCorrelationId();

            _logger.LogError(
                    ex,
                    "Unhandled exception occured. Method={Method}, Path={Path} CorrelationId={CorrelationId}",
                    context.Request.Method,
                    context.Request.Path,
                    correlationId);

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var response = new ApiErrorResponse
            {
                Error = "InternalServerError",
                Message = _environment.IsDevelopment()
                    ? ex.Message
                    : "An unexpected error occured.",
                CorrelationId = correlationId,
                TimestampUtc = DateTime.UtcNow
            };

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}
