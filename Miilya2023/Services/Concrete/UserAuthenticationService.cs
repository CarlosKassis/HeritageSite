using System.Threading.Tasks;

namespace Miilya2023.Services.Abstract
{
    public class UserAuthenticationService : IUserAuthenticationService
    {
        private const string _jwtStart = "Bearer ";

        public Task<bool> IsUserLoggedIn(string jwt)
        {
            if (jwt == null)
            {
                return Task.FromResult(false);
            }

            return Task.FromResult(jwt.StartsWith(_jwtStart) && jwt.Substring(_jwtStart.Length) == "ASD");
        }

        public Task TryLogin(string auth)
        {
            return Task.CompletedTask;
        }
    }
}
