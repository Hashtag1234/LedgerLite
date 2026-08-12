namespace LedgerLite.Api.Middleware;

// WHY: Correlation ID ties together logs across the entire request lifecycle.
// This is essential for troubleshooting distributed production requests.
public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;
    private const string CorrelationIdHeader = "X-Correlation-Id";
    private const string CorrelationIdContextKey = "CorrelationId";

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public Task InvokeAsync(HttpContext context)
    {
        // WHY: Use existing correlation ID from header, or generate a new one.
        string correlationId = context.Request.Headers.TryGetValue(CorrelationIdHeader, out var headerValue)
            ? headerValue.ToString()
            : Guid.NewGuid().ToString();

        context.Items[CorrelationIdContextKey] = correlationId;
        context.Response.Headers[CorrelationIdHeader] = correlationId;

        return _next(context);
    }
}
