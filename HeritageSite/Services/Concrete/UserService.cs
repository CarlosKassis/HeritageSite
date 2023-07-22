

namespace HeritageSite.Services.Abstract
{
    using Microsoft.Extensions.Logging;
    using HeritageSite.Constants;
    using MongoDB.Driver;
    using System;
    using System.Threading.Tasks;
    using static HeritageSite.Services.Utils.Documents;
    using Neo4j.Driver;

    public class UserService : IUserService
    {
        private static readonly IMongoClient _mongoClient = new MongoClient("mongodb://localhost:27017");
        private static readonly IMongoDatabase _database = _mongoClient.GetDatabase(PrivateHistoryConstants.DatabaseName);
        private static readonly IMongoCollection<UserDocument> _collection = _database.GetCollection<UserDocument>("Users");
        private readonly ILogger<UserAuthenticationService> _logger;
        private readonly IDriver _driver;

        public UserService(ILogger<UserAuthenticationService> logger, IDriver driver)
        {
            _logger = logger;
            _driver = driver;
        }

        public async Task<UserDocument> GetUserWithEmail(string email, bool createIfDoesntExist)
        {
            var filter = Builders<UserDocument>.Filter.Eq(x => x.Email, email);
            var userDocument = await _collection.Find(filter).Limit(1).FirstOrDefaultAsync();
            if (userDocument == null && createIfDoesntExist)
            {
                string userId = Guid.NewGuid().ToString();
                var newUserDocument = new UserDocument { Email = email, Id = userId };
                await _collection.InsertOneAsync(newUserDocument);
                return newUserDocument;
            }

            if (userDocument == null)
            {
                throw new InvalidOperationException("User doesn't exit");
            }

            return userDocument;
        }

        public async Task<UserDocument> GetUserWithId(string userId)
        {
            var filter = Builders<UserDocument>.Filter.Eq(x => x.Id, userId);
            var userDocument = await _collection.Find(filter).Limit(1).FirstOrDefaultAsync();
            if (userDocument == null)
            {
                throw new InvalidOperationException("User doesn't exit");
            }

            return userDocument;
        }
    }
}
