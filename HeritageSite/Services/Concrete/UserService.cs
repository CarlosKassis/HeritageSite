

namespace HeritageSite.Services.Abstract
{
    using Microsoft.Extensions.Logging;
    using HeritageSite.Constants;
    using MongoDB.Bson;
    using MongoDB.Driver;
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;
    using static HeritageSite.Services.Utils.Documents;

    public class UserService : IUserService
    {
        private static readonly IMongoClient _mongoClient = new MongoClient("mongodb://localhost:27017");
        private static readonly IMongoDatabase _database = _mongoClient.GetDatabase(PrivateHistoryConstants.DatabaseName);
        private static readonly IMongoCollection<UserDocument> _collection = _database.GetCollection<UserDocument>("Users");
        private static readonly SHA256 _sha256 = SHA256.Create();
        private ILogger<UserAuthenticationService> _logger;

        public UserService(ILogger<UserAuthenticationService> logger)
        {
            _logger = logger;
        }

        public async Task<UserDocument> GetUserWithLoginJwt(string jwt, bool createIfDoesntExist)
        {
            if (jwt == null)
            {
                throw new InvalidOperationException();
            }

            JwtSecurityToken token = new JwtSecurityToken(jwt);
            var email = token?.Audiences?.First()?.ToLower();
            if (email == null)
            {
                throw new InvalidOperationException("Token doesn't contain email");
            }

            return await GetUserWithEmail(email, createIfDoesntExist);
        }

        public async Task<UserDocument> GetUserWithEmail(string email, bool createIfDoesntExist)
        {
            var emailSHA256 = GetEmailBase64SHA256(email);
            return await GetUserWithEmailHash(emailSHA256, createIfDoesntExist);
        }

        private async Task<UserDocument> GetUserWithEmailHash(string emailSHA256, bool createIfDoesntExist)
        {
            var filter = Builders<UserDocument>.Filter.Eq(x => x.EmailSHA256, emailSHA256);
            var userDocument = await _collection.Find(filter).Limit(1).FirstOrDefaultAsync();
            if (userDocument == null && createIfDoesntExist)
            {
                await _collection.InsertOneAsync(new UserDocument { EmailSHA256 = emailSHA256, Id = ObjectId.GenerateNewId() });
                userDocument = await GetUserWithEmailHash(emailSHA256, createIfDoesntExist: false);
            }

            if (userDocument == null)
            {
                throw new InvalidOperationException("User doesn't exit");
            }

            return userDocument;
        }

        private static string GetEmailBase64SHA256(string email)
        {
            return Convert.ToBase64String(_sha256.ComputeHash(Encoding.UTF8.GetBytes(email.ToLower())));
        }
    }
}
