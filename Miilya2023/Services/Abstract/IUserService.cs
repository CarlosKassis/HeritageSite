

namespace Miilya2023.Services.Abstract
{
    using System.Threading.Tasks;
    using static Miilya2023.Services.Utils.Documents;

    public interface IUserService
    {
        Task<UserDocument> GetUserWithLoginJwt(string jwt, bool createIfDoesntExist = true);

        Task<UserDocument> GetUserWithEmail(string email, bool createIfDoesntExist = true);
    }
}
