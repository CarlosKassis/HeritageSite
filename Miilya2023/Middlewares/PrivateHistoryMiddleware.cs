
using Microsoft.AspNetCore.Http;
using Miilya2023.Services.Abstract;
using System;
using System.Threading.Tasks;

namespace Miilya2023.Middlewares
{

    public class PrivateHistoryMiddleware
    {
        private const string _privateHistoryPathStartSegment = "/PrivateHistory/";

        private readonly RequestDelegate _next;

        private readonly IUserAuthenticationService _userAuthenticationService;

        public PrivateHistoryMiddleware(RequestDelegate next, IUserAuthenticationService userAuthenticationService)
        {
            _next = next;
            _userAuthenticationService = userAuthenticationService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Path.Value.StartsWith(_privateHistoryPathStartSegment, StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }

            string jwtHeader = context.Request.Headers["Authorization"];

            bool userLoggedIn = await _userAuthenticationService.IsUserLoggedIn(jwtHeader);
            if (!userLoggedIn)
            {
                context.Response.StatusCode = 404;
                return;
            }

            await _next(context);
        }
    }
}
