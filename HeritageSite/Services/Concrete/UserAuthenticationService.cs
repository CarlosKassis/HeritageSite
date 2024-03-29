﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Google.Apis.Auth;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using static HeritageSite.Services.Abstract.Authentication;
using static HeritageSite.Services.Utils.Documents;

namespace HeritageSite.Services.Abstract
{
    public class UserAuthenticationService : IUserAuthenticationService
    {
        private const string _microsoftJwkAquireUrl = "https://login.microsoftonline.com/common/discovery/v2.0/keys";
        private const string _googleClientId = "774040641386-74otf6r69gv7nd92efvbh5kf5l6j8jf8.apps.googleusercontent.com";
        
        private const string _microsoftClientId = "3ee7d2ed-3ea7-4790-b63c-e06ccd058189";

        private static readonly SecurityKey _jwtSecurityKey = GetRsaCryptoServiceProviderKey();

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

        // Error-prone: make sure to refresh from database on data change
        private static readonly ConcurrentDictionary<string, UserDocument> _loginJwtsValidationResults = new ConcurrentDictionary<string, UserDocument>();

        private static Task _microsoftJwkRetrieverDaemon;

        private static IEnumerable<JsonWebKey> _microsoftJsonWebKeys;

        private static readonly JwtSecurityTokenHandler _microsoftTokenHandler = new JwtSecurityTokenHandler();

        private readonly IUserService _userService;

        private readonly ILogger<UserAuthenticationService> _logger;

        public UserAuthenticationService(IUserService userService, ILogger<UserAuthenticationService> logger)
        {
            _userService = userService;
            _logger = logger;
            _microsoftJwkRetrieverDaemon ??= Task.Factory.StartNew(async () => await RetrieveMicrosoftJsonWebKeysLoop(logger));
        }

        public async Task<UserDocument> ValidateLoginJwtAndGetUser(string jwt)
        {
            if (jwt == null)
            {
                throw new InvalidOperationException("Token is empty");
            }

            if (_loginJwtsValidationResults.TryGetValue(jwt, out var user))
            {
                return user;
            }

            var validationResult = await _tokenHandler.ValidateTokenAsync(jwt, _tokenValidationParameters);
            if (!validationResult.IsValid)
            {
                throw new InvalidOperationException("Login token is invalid");
            }

            if (!validationResult.Claims.TryGetValue("login", out var loginValue) || loginValue as string != "login")
            {
                throw new InvalidOperationException("Invalid token content");
            }

            string userId = validationResult.Claims["aud"] as string;
            if (string.IsNullOrEmpty(userId))
            {
                throw new InvalidOperationException("UserId is empty");
            }

            user = await _userService.GetUserWithId(userId);

            _loginJwtsValidationResults.TryAdd(jwt, user);
            return user;
        }

        public async Task<string> CreateSiteLoginkJwtFromCustomLogin(string email, string password)
        {
            // TODO: add email+password check

            var user = await _userService.GetUserWithEmail(email);
            return CreateLoginJwtForUser(user.Id);
        }

        public async Task<string> CreateSiteLoginJwtFromThirdPartyLoginJwt(string jwt, AccountAuthentication accountAuthentication)
        {
            string email = accountAuthentication switch
            {
                AccountAuthentication.Google => await ValidateGoogleLoginJwtAndGetEmail(jwt),
                AccountAuthentication.Microsoft => ValidateMicrosoftLoginJwtAndGetEmail(jwt),
                _ => throw new NotSupportedException("Only Custom, Microsoft and Google login is supported")
            };

            var user = await _userService.GetUserWithEmail(email);
            return CreateLoginJwtForUser(user.Id);
        }

        private string CreateLoginJwtForUser(string userId)
        {
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                SigningCredentials = _jwtSigningCredentials,
                Subject = new ClaimsIdentity(),
                Claims = new Dictionary<string, object>
                {
                    { "login", "login" }
                },
                Expires = DateTime.UtcNow.AddYears(1),
                NotBefore = DateTime.UtcNow,
                IssuedAt = DateTime.UtcNow,
                Audience = userId
            };

            return _tokenHandler.CreateEncodedJwt(tokenDescriptor);
        }

        private string ValidateMicrosoftLoginJwtAndGetEmail(string jwt)
        {
            if (_microsoftJsonWebKeys == null)
            {
                _logger.LogError($"Microsoft validation keys aren't available. {nameof(_microsoftJsonWebKeys)} is null");
                throw new Exception("Microsoft validation keys aren't available");
            }

            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(jwt);

            // Log the JWKs
            foreach (JsonWebKey jwk in _microsoftJsonWebKeys)
            {
                try
                {
                    TokenValidationParameters validationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = jwtSecurityToken.Issuer,
                        ValidAudience = jwtSecurityToken.Audiences.First(),
                        IssuerSigningKey = jwk,
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,
                    };

                    ClaimsPrincipal claimsPrincipal = _microsoftTokenHandler.ValidateToken(jwt, validationParameters, out SecurityToken validatedToken);

                    if (claimsPrincipal.Claims.FirstOrDefault(claim => claim.Type == "aud")?.Value != _microsoftClientId)
                    {
                        throw new Exception("Invalid application ID login");
                    }

                    // Extract the claims from the validated token
                    return claimsPrincipal.Claims.FirstOrDefault(claim => claim.Type == "preferred_username")?.Value;
                }
                catch (Exception)
                {
                }
            }

            throw new Exception("Login is invalid");
        }

        private async Task<string> ValidateGoogleLoginJwtAndGetEmail(string jwt)
        {
            GoogleJsonWebSignature.Payload payload = await GoogleJsonWebSignature.ValidateAsync(jwt);

            if (!payload.Audience.Equals(_googleClientId))
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

        private static SecurityKey GetRsaCryptoServiceProviderKey()
        {
            // Temporary way to store RSA key for development purposes
            // TODO: use a different method to store keys
            string rsaFile = Path.Combine(".", "RSA.txt");
            if (File.Exists(rsaFile))
            {
                string rsaJson = File.ReadAllText(rsaFile);
                var rsaParameters = JsonConvert.DeserializeObject<RSAParameters>(rsaJson);
                return new RsaSecurityKey(rsaParameters);
            }
            else
            {
                var rsaProvider = new RSACryptoServiceProvider(512);
                var rsaParameters = rsaProvider.ExportParameters(true);
                File.WriteAllText(rsaFile, JsonConvert.SerializeObject(rsaParameters));
                return new RsaSecurityKey(rsaParameters);
            }
        }

        private static async Task RetrieveMicrosoftJsonWebKeysLoop(ILogger<UserAuthenticationService> logger)
        {
            while (true)
            {
                try
                {
                    using HttpClient httpClient = new HttpClient();
                    using HttpResponseMessage httpResponse = await httpClient.GetAsync(_microsoftJwkAquireUrl);

                    if (!httpResponse.IsSuccessStatusCode)
                    {
                        throw new Exception($"Failed to retrieve JWKs. HTTP status code: {httpResponse.StatusCode}");
                    }

                    string responseJson = await httpResponse.Content.ReadAsStringAsync();
                    JsonWebKeySet jwks = new JsonWebKeySet(responseJson);
                    _microsoftJsonWebKeys = jwks.Keys;
                    await Task.Delay(TimeSpan.FromHours(1));
                }
                catch (Exception ex)
                {
                    logger?.LogError(ex.Message, ex);
                    await Task.Delay(TimeSpan.FromMinutes(10));
                }
            }
        }
    }
}
