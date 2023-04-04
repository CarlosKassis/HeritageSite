
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace Miilya2023.Middlewares
{

    public class PrivateHistoryMiddleware
    {
        private readonly RequestDelegate _next;

        public PrivateHistoryMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            Console.WriteLine(context.Request.Path.Value);
            // Call the next delegate/middleware in the pipeline.
            await _next(context);
        }
    }
}
