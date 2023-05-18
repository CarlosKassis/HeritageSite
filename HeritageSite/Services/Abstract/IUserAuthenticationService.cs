
namespace HeritageSite.Services.Abstract
{
    using System.Threading.Tasks;
    using static HeritageSite.Services.Abstract.Authentication;
    using static HeritageSite.Services.Utils.Documents;

    public interface IUserAuthenticationService
    {
        public Task<string> CreateSiteLoginJwtFromThirdPartyLoginJwt(string jwt, AccountAuthentication accountAuthentication);

        public Task<UserDocument> ValidateLoginJwtAndGetUser(string jwt);
    }
}
