﻿namespace Miilya2023.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Miilya2023.Services.Abstract;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using static Miilya2023.Services.Abstract.Authentication;

    [ApiController]
    [Route("[Controller]")]
    public class UserAuthenticationController : ControllerBase
    {
        private const string _jwtHeaderKey = "Authorization";

        private const string _googleCredentialHeaderKey = "google-credentials";

        private const string _microsoftCredentialHeaderKey = "microsoft-credentials";

        private readonly IUserAuthenticationService _userAuthenticationService;

        public UserAuthenticationController(IUserAuthenticationService userAuthenticationService)
        {
            _userAuthenticationService = userAuthenticationService;
        }

        /// <summary>
        /// Receives a Google login response from header and returns a login JWT
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("Google/Login")]
        public async Task<IActionResult> GoogleLogin()
        {
            if (!Request.Headers.TryGetValue(_googleCredentialHeaderKey, out var googleCredentials))
            {
                throw new ArgumentException("Received Google login validation without response credentials");
            }

            var googleJwt = googleCredentials.First();
            string loginJwt = await _userAuthenticationService.CreateSiteLoginJwtFromThirdPartyLoginJwt(googleJwt, AccountAuthentication.Google);

            return Content(loginJwt);
        }

        /// <summary>
        /// Receives a Microsoft login response from header and returns a login JWT
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("Microsoft/Login")]
        public async Task<IActionResult> MicrosoftLogin()
        {
            if (!Request.Headers.TryGetValue(_microsoftCredentialHeaderKey, out var microsoftCredentials))
            {
                throw new ArgumentException("Received Microsoft login validation without response credentials");
            }

            var microsoftJwt = microsoftCredentials.First();
            string loginJwt = await _userAuthenticationService.CreateSiteLoginJwtFromThirdPartyLoginJwt(microsoftJwt, AccountAuthentication.Microsoft);

            return Content(loginJwt);
        }

        /// <summary>
        /// Validates login JWT from header
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Validate")]
        public async Task<IActionResult> ValidateJwt()
        {
            var jwt = Request.Headers["Authorization"].First();
            if (jwt == null)
            {
                throw new InvalidOperationException("Received validation request without token");
            }

            await _userAuthenticationService.ValidateLoginJwtAndGetUser(jwt);
            return Ok();
        }

    }
}
