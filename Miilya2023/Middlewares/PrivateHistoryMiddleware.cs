
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.HttpSys;
using Miilya2023.Constants;
using Miilya2023.Services.Abstract;
using System;
using System.Threading.Tasks;

namespace Miilya2023.Middlewares
{

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
            string jwtHeader = context.Request.Headers["Authorization"];

            bool userLoggedIn = await _userAuthenticationService.IsUserLoggedIn(jwtHeader);
            if (!userLoggedIn)
            {
                //context.Response.StatusCode = 404;
                //return;
            }

            await _next(context);
        }
    }
}
