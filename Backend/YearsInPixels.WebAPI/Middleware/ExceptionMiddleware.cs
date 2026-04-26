namespace YearsInPixels.WebAPI.Middleware
{
    using FluentValidation;
    using System.Net;
    using System.Text.Json;
    using YearsInPixels.Application.DTOs;

    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
                _logger.LogError(ex, "An unhandled exception occurred.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = exception switch
            {
                ValidationException ve => new ErrorResponse
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = "Validation failed",
                    Errors = ve.Errors.Select(e => e.ErrorMessage)
                },

                _ => new ErrorResponse
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = "Internal Server Error. Please try again later."
                }
            };

            context.Response.StatusCode = response.StatusCode;

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            return context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
        }
    }
}
