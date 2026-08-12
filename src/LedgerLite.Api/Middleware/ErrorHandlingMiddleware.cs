using System.Net;
using System.Text.Json;

namespace LedgerLite.Api.Middleware;

// WHY: Centralized error handling returns RFC7807 ProblemDetails for all exceptions.
// This keeps error responses consistent and machine-readable.
public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }
    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        // WHY: ProblemDetails (RFC7807) is a standard way to communicate errors
        // in REST APIs. Clients can parse it consistently.
        var problemDetails = new ProblemDetails
        {
            Status = context.Response.StatusCode,
            Title = "An unexpected error occurred.",
            Detail = exception.Message ?? "An error occurred.",
            Instance = context.Request.Path.Value ?? "/"
        };

        string? correlationIdStr = null;
        if (context.Items.TryGetValue("CorrelationId", out var correlationId) && correlationId != null)
        {
            correlationIdStr = correlationId.ToString();
            problemDetails.Extensions["traceId"] = correlationId;
        }

        _logger.LogError(exception, "Unhandled exception. Path={Path}, CorrelationId={CorrelationId}", context.Request.Path, correlationIdStr);

        return context.Response.WriteAsJsonAsync(problemDetails);
    }
}

// WHY: ProblemDetails follows RFC7807, the standard for HTTP API error responses.
public class ProblemDetails
{
    public int? Status { get; set; }
    public string? Title { get; set; }
    public string? Detail { get; set; }
    public string? Instance { get; set; }
    public Dictionary<string, object> Extensions { get; set; } = new();
}
