

namespace HeritageSite.Services.Abstract
{
    using Microsoft.Extensions.Logging;
    using HeritageSite.Constants;
    using MongoDB.Driver;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using static HeritageSite.Services.Utils.Documents;

    public class BookmarkService : IBookmarkService
    {
        private static readonly IMongoClient _mongoClient = new MongoClient("mongodb://localhost:27017");
        private static readonly IMongoDatabase _database = _mongoClient.GetDatabase(PrivateHistoryConstants.DatabaseName);
        private static readonly IMongoCollection<BookmarkDocument> _collection = _database.GetCollection<BookmarkDocument>("Bookmarks");
        private ILogger<UserAuthenticationService> _logger;

        public BookmarkService(ILogger<UserAuthenticationService> logger)
        {
            _logger = logger;
        }

        public async Task<IEnumerable<BookmarkDocument>> GetUserBookmarks(UserDocument user)
        {
            var filter = Builders<BookmarkDocument>.Filter.Eq(x => x.UserId, user.Id);
            var bookmarks = await _collection.Find(filter).ToListAsync();
            return bookmarks;
        }

        public async Task AddBookmark(UserDocument user, int historyPostIndex)
        {
            var filter = Builders<BookmarkDocument>.Filter.Eq(x => x.UserId, user.Id)
                & Builders<BookmarkDocument>.Filter.Eq(x => x.HistoryPostIndex, historyPostIndex);

            var update = Builders<BookmarkDocument>.Update
                            .Set(x => x.UserId, user.Id)
                            .Set(x => x.HistoryPostIndex, historyPostIndex);

            await _collection.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true });
        }

        public async Task RemoveBookmark(UserDocument user, int historyPostIndex)
        {
            var filter = Builders<BookmarkDocument>.Filter.Eq(x => x.UserId, user.Id)
                & Builders<BookmarkDocument>.Filter.Eq(x => x.HistoryPostIndex, historyPostIndex);

            await _collection.DeleteManyAsync(filter);
        }

        public async Task DeleteBookmarksForHistoryPost(int historyPostIndex)
        {
            var filter = Builders<BookmarkDocument>.Filter.Eq(x => x.HistoryPostIndex, historyPostIndex);
            await _collection.DeleteManyAsync(filter);
        }
    }
}
