using System.Threading.Tasks;

namespace Miilya2023.Services.Abstract
{
    public interface IUserAuthenticationService
    {
        public Task TryLogin(string auth);

        public Task<bool> IsUserLoggedIn(string jwt);
    }
}
