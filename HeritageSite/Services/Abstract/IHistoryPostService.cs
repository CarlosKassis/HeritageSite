
namespace HeritageSite.Services.Abstract
{
    using SixLabors.ImageSharp;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using static HeritageSite.Services.Utils.Documents;

    public interface IHistoryPostService
    {
        public Task<List<HistoryPostDocument>> GetFirstBatchLowerEqualThanIndex(int? index, int batchSize, string searchText = null);

        public Task InsertHistoryPost(string userId, string title, string description, Image image, DateOnly? imageDate);

        public Task DeleteHistoryPost(string userId, long index, bool isAdmin);

        public Task CreateGraphUserIfDoesntExist(string userId);

        public Task<IEnumerable<long>> GetUserBookmarkPostIndexes(string userId);

        public Task RemoveBookmark(string userId, int historyPostIndex);

        public Task AddBookmark(string userId, int historyPostIndex);
    }
}
