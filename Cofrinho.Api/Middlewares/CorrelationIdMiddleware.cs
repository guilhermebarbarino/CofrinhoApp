using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Cofrinho.Api.Middlewares;

[ExcludeFromCodeCoverage]

public sealed class CorrelationIdMiddleware
{
    public const string HeaderName = "X-Correlation-Id";

    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var correlationId = context.Request.Headers.TryGetValue(HeaderName, out var headerValue)
            && !string.IsNullOrWhiteSpace(headerValue)
                ? headerValue.ToString()
                : Guid.NewGuid().ToString("N");

        // Guarda no HttpContext para qualquer lugar acessar
        context.Items[HeaderName] = correlationId;

        // Garante que volta no response
        context.Response.OnStarting(() =>
        {
            context.Response.Headers[HeaderName] = correlationId;
            return Task.CompletedTask;
        });

        await _next(context);
    }
}
