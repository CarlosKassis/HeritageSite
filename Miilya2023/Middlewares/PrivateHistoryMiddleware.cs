
namespace Miilya2023.Middlewares
{
    using Microsoft.AspNetCore.Http;
    using Miilya2023.Constants;
    using Miilya2023.Services.Abstract;
    using System.Threading.Tasks;

    public class PrivateHistoryMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly IUserAuthenticationService _userAuthenticationService;

        public PrivateHistoryMiddleware(RequestDelegate next, IUserAuthenticationService userAuthenticationService)
        {
            _next = next;
            _userAuthenticationService = userAuthenticationService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Path.HasValue || !context.Request.Path.Value.StartsWith(PrivateHistoryConstants.UrlPrefix))
            {
                await _next(context);
                return;
            }

            string jwtHeader = context.Request.Headers["Authorization"];
            bool userLoggedIn = await _userAuthenticationService.IsLoginJwtValid(jwtHeader);
            if (!userLoggedIn)
            {
                context.Response.StatusCode = 404;
                return;
            }

            await _next(context);
        }
    }
}
