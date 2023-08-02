using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace HeritageSite.Middlewares
{
    public class CustomExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public CustomExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                await SetResponseAccordingToException(context, ex);
            }
        }

        private async Task SetResponseAccordingToException(HttpContext context, Exception exception)
        {
            context.Response.StatusCode = exception switch
            {
                InvalidOperationException => StatusCodes.Status403Forbidden,
                _ => StatusCodes.Status500InternalServerError
            };

            await context.Response.WriteAsync(exception?.Message ?? "Unknown error");
        }
    }
}
