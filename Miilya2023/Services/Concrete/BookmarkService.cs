

namespace Miilya2023.Services.Abstract
{
    using Microsoft.Extensions.Logging;
    using Miilya2023.Constants;
    using MongoDB.Driver;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using static Miilya2023.Services.Utils.Documents;

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

        public async Task<BookmarkDocument> GetUserBookmarks(UserDocument user)
        {
            var filter = Builders<BookmarkDocument>.Filter.Eq(x => x.UserId, user.Id);
            var bookmarks = (await _collection.Find(filter).FirstOrDefaultAsync());
            return bookmarks;
        }

        public async Task AddBookmark(UserDocument user, int historyPostIndex)
        {
            var filter = Builders<BookmarkDocument>.Filter.Eq(x => x.UserId, user.Id);
            var bookmarks = await _collection.Find(filter).FirstOrDefaultAsync();
            if (bookmarks != null)
            {
                if (bookmarks.BookmarkedHistoryPostsIndexes == null)
                {
                    bookmarks.BookmarkedHistoryPostsIndexes = new HashSet<int> { historyPostIndex };
                }
                else
                {
                    bookmarks.BookmarkedHistoryPostsIndexes.Add(historyPostIndex);
                }

                await _collection.ReplaceOneAsync(filter, bookmarks);
                return;
            }

            await _collection.InsertOneAsync(new BookmarkDocument { UserId = user.Id, BookmarkedHistoryPostsIndexes = new HashSet<int> { historyPostIndex } });
        }

        public async Task RemoveBookmark(UserDocument user, int historyPostIndex)
        {
            var filter = Builders<BookmarkDocument>.Filter.Eq(x => x.UserId, user.Id);
            var bookmarks = await _collection.Find(filter).FirstOrDefaultAsync();
            if (bookmarks != null)
            {
                if (bookmarks.BookmarkedHistoryPostsIndexes != null)
                {
                    bookmarks.BookmarkedHistoryPostsIndexes.Remove(historyPostIndex);
                }

                await _collection.ReplaceOneAsync(filter, bookmarks);
                return;
            }
        }
    }
}
