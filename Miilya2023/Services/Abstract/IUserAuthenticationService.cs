
namespace Miilya2023.Services.Abstract
{
    using System.Threading.Tasks;
    using static Miilya2023.Services.Abstract.Authentication;
    using static Miilya2023.Services.Utils.Documents;

    public interface IUserAuthenticationService
    {
        public Task<string> CreateSiteLoginJwtFromThirdPartyLoginJwt(string jwt, AccountAuthentication accountAuthentication);

        public Task<UserDocument> ValidateLoginJwtAndGetUser(string jwt);
    }
}
