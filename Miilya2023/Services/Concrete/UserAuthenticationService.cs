using System;
using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Auth;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using Miilya2023.Constants;
using Newtonsoft.Json;
using static Miilya2023.Services.Abstract.Authentication;

namespace Miilya2023.Services.Abstract
{
    public class UserAuthenticationService : IUserAuthenticationService
    {
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
            string email = null;
            email = accountAuthentication switch
            {
                AccountAuthentication.Google => await ValidateGoogleLoginJwt(jwt),
                AccountAuthentication.Microsoft => await ValidateMicrosoftLoginJwt(jwt),
                _ => throw new NotSupportedException("Only Microsoft or Google accounts are supported")
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

        private static readonly IConfidentialClientApplication _microsoftConfidentialClientApplication = ConfidentialClientApplicationBuilder
                .Create(_microsoftClientId)
                .WithTenantId("10f100d7-a82a-4e12-8033-7d4c66a96a04")
                .WithClientSecret(AuthenticationConstants.MicrosoftClientSecret)
                .Build();

        private async Task<string> ValidateMicrosoftLoginJwt(string jwt)
        {
            try
            {
                return "asd";
            }
            catch (MsalUiRequiredException ex)
            {
                Console.WriteLine(ex);
            }
            catch (MsalException ex)
            {
                Console.WriteLine(ex);
            }

            return "asd";
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
    }
}
