using Serilog.Context;
using System.Diagnostics;

namespace Cofrinho.Api.Middlewares;

public sealed class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        var sw = Stopwatch.StartNew();

        var correlationId = context.Items.TryGetValue(
                CorrelationIdMiddleware.HeaderName,
                out var cid)
            ? cid?.ToString()
            : null;

        try
        {
            await _next(context);
            sw.Stop();

            using (LogContext.PushProperty("Method", context.Request.Method))
            using (LogContext.PushProperty("Path", context.Request.Path.Value))
            using (LogContext.PushProperty("StatusCode", context.Response.StatusCode))
            using (LogContext.PushProperty("ElapsedMs", sw.ElapsedMilliseconds))
            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                _logger.LogInformation("request");
            }
        }
        catch
        {
            sw.Stop();

            using (LogContext.PushProperty("Method", context.Request.Method))
            using (LogContext.PushProperty("Path", context.Request.Path.Value))
            using (LogContext.PushProperty("StatusCode", context.Response.StatusCode))
            using (LogContext.PushProperty("ElapsedMs", sw.ElapsedMilliseconds))
            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                _logger.LogError("request_failed");
            }

            throw;
        }
    }



}
