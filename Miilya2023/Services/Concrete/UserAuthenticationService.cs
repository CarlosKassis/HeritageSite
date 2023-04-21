using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using Miilya2023.Constants;
using Newtonsoft.Json;
using static Miilya2023.Services.Abstract.Authentication;

namespace Miilya2023.Services.Abstract
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

        private static readonly ConcurrentDictionary<string, bool> _loginJwtsValidationResults = new ConcurrentDictionary<string, bool>();

        private static Task _microsoftJwkRetrieverDaemon;

        private static IEnumerable<JsonWebKey> _microsoftJsonWebKeys;

        private static readonly JwtSecurityTokenHandler _microsoftTokenHandler = new JwtSecurityTokenHandler();

        private ILogger<UserAuthenticationService> _logger;

        public UserAuthenticationService(ILogger<UserAuthenticationService> logger)
        {
            _logger = logger;
            _microsoftJwkRetrieverDaemon ??= Task.Factory.StartNew(() => RetrieveMicrosoftJsonWebKeysLoop(logger));
        }

        public async Task<bool> IsLoginJwtValid(string jwt)
        {
            if (jwt == null)
            {
                return false;
            }

            if (_loginJwtsValidationResults.TryGetValue(jwt, out bool valid))
            {
                return valid;
            }

            var validationResult = await _tokenHandler.ValidateTokenAsync(jwt, _tokenValidationParameters);
            _loginJwtsValidationResults.TryAdd(jwt, validationResult.IsValid);

            return validationResult.IsValid;
        }


        public async Task<string> CreateSiteLoginJwtFromThirdPartyLoginJwt(string jwt, AccountAuthentication accountAuthentication)
        {
            string email = accountAuthentication switch
            {
                AccountAuthentication.Google => await ValidateGoogleLoginJwt(jwt),
                AccountAuthentication.Microsoft => ValidateMicrosoftLoginJwt(jwt),
                _ => throw new NotSupportedException("Only Microsoft and Google accounts are supported")
            };

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

        private string ValidateMicrosoftLoginJwt(string jwt)
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

        private async Task<string> ValidateGoogleLoginJwt(string jwt)
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
            string rsaFile = ".\\RSA.txt";
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
