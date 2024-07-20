namespace SmartKeyCaddy.Api.ExceptionHandling
{
    public static class ExceptionMiddlewareExtentions
    {
        public static void ConfigureCustomExceptionMiddleware(this WebApplication app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
