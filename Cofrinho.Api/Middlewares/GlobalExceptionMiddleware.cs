namespace Cofrinho.Api.Middlewares
{
    using System.Net;
    using Microsoft.AspNetCore.Mvc;

    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
                _logger.LogError(ex, "Unhandled error. Path={Path}", context.Request.Path);

                var (status, title) = ex switch
                {
                    ArgumentException => (HttpStatusCode.BadRequest, ex.Message),
                    InvalidOperationException => (HttpStatusCode.Conflict, ex.Message),
                    KeyNotFoundException => (HttpStatusCode.NotFound, ex.Message),
                    _ => (HttpStatusCode.InternalServerError, "Ocorreu um erro inesperado.")
                };

                var problem = new ProblemDetails
                {
                    Status = (int)status,
                    Title = title
                };

                context.Response.StatusCode = (int)status;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(problem);
            }
        }
    }

}
