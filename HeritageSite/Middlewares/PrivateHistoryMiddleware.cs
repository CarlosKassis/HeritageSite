
namespace HeritageSite.Middlewares
{
    using Microsoft.AspNetCore.Http;
    using HeritageSite.Constants;
    using HeritageSite.Services.Abstract;
    using System;
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

            try
            {
                string loginJwt = context.Request.Headers["Authorization"];
                var user = await _userAuthenticationService.ValidateLoginJwtAndGetUser(loginJwt);
                context.Items["User"] = user ?? throw new Exception("Unable to get user");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                context.Response.StatusCode = 403;
                return;
            }


            await _next(context);
        }
    }
}
