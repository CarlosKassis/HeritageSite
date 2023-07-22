

namespace HeritageSite.Services.Abstract
{
    using System.Threading.Tasks;
    using static HeritageSite.Services.Utils.Documents;

    public interface IUserService
    {
        Task<UserDocument> GetUserWithEmail(string email, bool createIfDoesntExist = true);

        Task<UserDocument> GetUserWithId(string userId);
    }
}
