
namespace Miilya2023.Middlewares
{
    using Microsoft.AspNetCore.Http;
    using Miilya2023.Constants;
    using Miilya2023.Services.Abstract;
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Threading.Tasks;

    public class PrivateHistoryMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly IUserAuthenticationService _userAuthenticationService;

        private readonly IUserService _userService;

        public PrivateHistoryMiddleware(RequestDelegate next, IUserAuthenticationService userAuthenticationService, IUserService userService)
        {
            _next = next;
            _userAuthenticationService = userAuthenticationService;
            _userService = userService;
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
                context.Items["User"] = await _userAuthenticationService.ValidateLoginJwtAndGetUser(loginJwt);
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
