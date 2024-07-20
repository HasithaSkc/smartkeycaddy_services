using SmartKeyCaddy.Models.Exceptions;
using System.Net;

namespace SmartKeyCaddy.Api.ExceptionHandling
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        public ExceptionMiddleware(RequestDelegate next, ILogger logger)
        {
            _logger = logger;
            _next = next;
        }
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                await HandleExceptionAsync(httpContext, ex);
            }
        }
        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var exceptionDetails = GetExceptionDetails(exception);
            context.Response.StatusCode = exceptionDetails.StatusCode;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(new ExceptionDetails()
            {
                StatusCode = exceptionDetails.StatusCode,
                Message = exceptionDetails.Message
            }.ToString());
        }

        private ExceptionDetails GetExceptionDetails(Exception exception)
        {
            switch (exception)
            {
                case UnauthorizedAccessException:
                    {
                        return new ExceptionDetails()
                        {
                            StatusCode = (int)HttpStatusCode.Unauthorized,
                            Message = $"Unauthorized access: {exception.Message}"
                        };
                        
                    }
                case NotFoundException:
                    {
                        return new ExceptionDetails()
                        {
                            StatusCode = (int)HttpStatusCode.NotFound,
                            Message = $"{exception.Message}"
                        };

                    }
                default:
                    return new ExceptionDetails()
                    {
                        StatusCode = (int)HttpStatusCode.InternalServerError,
                        Message = $"Internal Server Error: {exception.Message}"
                    };
            }
        }
    }
}
