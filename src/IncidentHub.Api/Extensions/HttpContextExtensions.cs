namespace IncidentHub.Api.Extensions;

public static class HttpContextExtensions
{
    private const string HeaderName = "X-Correlation-ID";

    public static string GetCorrelationId(this HttpContext context)
    {
        return context.Items.TryGetValue(HeaderName, out var correlationId)
            ? correlationId?.ToString() ?? ""
            : "";
    }
}
