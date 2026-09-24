using System.Net;
using System.Text.Json;
using EduRate.Application.Common.Exceptions;
using ApplicationValidationException = EduRate.Application.Common.Exceptions.ValidationException;

namespace EduRate.API.Middleware
{
    /// <summary>
    /// Single place that turns exceptions thrown anywhere in the Application layer into HTTP
    /// responses, replacing the try/catch-per-action style the original controllers didn't
    /// consistently have. Maps each Application exception type to the same status code the
    /// original controllers used for that situation (BadRequest -&gt; 400, NotFound -&gt; 404, etc).
    /// </summary>
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            object payload;

            switch (exception)
            {
                case ApplicationValidationException validationEx:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    payload = new { message = "Validation failed.", errors = validationEx.Errors };
                    break;

                case BadRequestException badRequestEx:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    payload = new { message = badRequestEx.Message };
                    break;

                case NotFoundException notFoundEx:
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    payload = new { message = notFoundEx.Message };
                    break;

                case UnauthorizedException unauthorizedEx:
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    payload = new { message = unauthorizedEx.Message };
                    break;

                case ForbiddenAccessException forbiddenEx:
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    payload = new { message = forbiddenEx.Message };
                    break;

                default:
                    _logger.LogError(exception, "Unhandled exception");
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    payload = new { message = "An unexpected error occurred. Please try again later." };
                    break;
            }

            var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            await context.Response.WriteAsync(json);
        }
    }
}
