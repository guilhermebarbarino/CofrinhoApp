namespace Cofrinho.Api.Middlewares
{
    using System.Net;
    using Microsoft.AspNetCore.Mvc;

    public sealed class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var correlationId = context.Items.TryGetValue(
                        CorrelationIdMiddleware.HeaderName,
                        out var cid)
                    ? cid?.ToString()
                    : null;

                var (status, title, logLevel) = ex switch
                {
                    ArgumentException => (
                        HttpStatusCode.BadRequest,
                        ex.Message,
                        LogLevel.Warning
                    ),

                    InvalidOperationException => (
                        HttpStatusCode.Conflict,
                        ex.Message,
                        LogLevel.Warning
                    ),

                    KeyNotFoundException => (
                        HttpStatusCode.NotFound,
                        ex.Message,
                        LogLevel.Warning
                    ),

                    _ => (
                        HttpStatusCode.InternalServerError,
                        "Ocorreu um erro inesperado.",
                        LogLevel.Error
                    )
                };

                _logger.Log(
                    logLevel,
                    ex,
                    "Request failed. Path={Path} CorrelationId={CorrelationId}",
                    context.Request.Path,
                    correlationId
                );

                var problem = new ProblemDetails
                {
                    Status = (int)status,
                    Title = title,
                    Instance = context.Request.Path
                };

                problem.Extensions["correlationId"] = correlationId;

                context.Response.StatusCode = (int)status;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsJsonAsync(problem);
            }
        }
    }
}
