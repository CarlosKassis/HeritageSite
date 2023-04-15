﻿using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Amazon.SecurityToken.Model;
using Google.Apis.Auth;
using Microsoft.IdentityModel.Tokens;
using static Miilya2023.Services.Abstract.Authentication;

namespace Miilya2023.Services.Abstract
{
    public class UserAuthenticationService : IUserAuthenticationService
    {
        private const string _googleAppId = "774040641386-74otf6r69gv7nd92efvbh5kf5l6j8jf8.apps.googleusercontent.com";

        private static readonly SecurityKey _jwtSecurityKey = GenerateRsaCryptoServiceProviderKey();

        private static readonly SigningCredentials _jwtSigningCredentials = new SigningCredentials(_jwtSecurityKey, SecurityAlgorithms.RsaSha256);

        private static readonly JwtSecurityTokenHandler _tokenHandler = new JwtSecurityTokenHandler
            {
                SetDefaultTimesOnTokenCreation = false
            };

        private static readonly TokenValidationParameters _tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _jwtSecurityKey,
                ValidateIssuer = false,
                ValidateAudience = false
            };

        public async Task<bool> IsLoginJwtValid(string jwt)
        {
            if (jwt == null)
            {
                return false;
            }

            var validationResult = await _tokenHandler.ValidateTokenAsync(jwt, _tokenValidationParameters);
            return validationResult.IsValid;
        }


        public async Task<string> CreateSiteLoginJwtFromThirdPartyLoginJwt(string jwt, AccountAuthentication accountAuthentication)
        {
            string email = null;
            if (accountAuthentication == AccountAuthentication.Google)
            {
                email = await ValidateGoogleLoginJwt(jwt);
            }
            else
            {
                throw new NotSupportedException("Only Google accounts are currently supported");
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                SigningCredentials = _jwtSigningCredentials,
                Subject = new ClaimsIdentity(),
                Expires = DateTime.UtcNow.AddYears(1),
                NotBefore = DateTime.UtcNow,
                IssuedAt = DateTime.UtcNow,
                Audience = email
            };

            return _tokenHandler.CreateEncodedJwt(tokenDescriptor);
        }

        private async Task<string> ValidateGoogleLoginJwt(string jwt)
        {
            GoogleJsonWebSignature.Payload payload = await GoogleJsonWebSignature.ValidateAsync(jwt);

            if (!payload.Audience.Equals(_googleAppId))
            {
                throw new ArgumentException("Invalid Google App ID");
            }

            if (!payload.Issuer.Equals("accounts.google.com") && !payload.Issuer.Equals("https://accounts.google.com"))
            {
                throw new ArgumentException($"Invalid issuer: '{payload.Issuer}'");
            }

            if (payload.ExpirationTimeSeconds == null)
            {
                throw new ArgumentException($"Empty token expiration");
            }

            DateTime now = DateTime.Now.ToUniversalTime();
            DateTime expiration = DateTimeOffset.FromUnixTimeSeconds((long)payload.ExpirationTimeSeconds).DateTime;
            if (now > expiration)
            {
                throw new ArgumentException($"Token has expired");
            }

            return payload.Email;
        }

        private static SecurityKey GenerateRsaCryptoServiceProviderKey()
        {
            var rsaProvider = new RSACryptoServiceProvider(512);
            SecurityKey key = new RsaSecurityKey(rsaProvider);
            return key;
        }
    }
}
