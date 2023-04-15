
namespace Miilya2023.Services.Abstract
{
    using System.Threading.Tasks;
    using static Miilya2023.Services.Abstract.Authentication;

    public interface IUserAuthenticationService
    {
        public Task<string> CreateSiteLoginJwtFromThirdPartyLoginJwt(string jwt, AccountAuthentication accountAuthentication);

        public Task<bool> IsLoginJwtValid(string jwt);
    }
}
