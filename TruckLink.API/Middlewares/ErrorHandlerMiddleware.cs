namespace TruckLink.API.middlewares
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Net;
    using System.Text.Json;
    using System.Threading.Tasks;

    namespace TruckLink.API.Middleware
    {
        public class ErrorHandlerMiddleware
        {
            private readonly RequestDelegate _next;
            private readonly ILogger<ErrorHandlerMiddleware> _logger;

            public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
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
                catch (Exception error)
                {
                    _logger.LogError(error, "Unhandled Exception");

                    var response = context.Response;
                    response.ContentType = "application/json";

                    // Default to 500 if not handled explicitly
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;

                    var result = JsonSerializer.Serialize(new
                    {
                        message = "Something went wrong. Please try again later.",
                        error = error.Message // 🔒 remove this in production
                    });

                    await response.WriteAsync(result);
                }
            }
        }
    }

}
