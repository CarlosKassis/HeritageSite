﻿
namespace HeritageSite.Services.Abstract
{
    using SixLabors.ImageSharp;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using static HeritageSite.Services.Utils.Documents;

    public interface IHistoryPostService
    {
        public Task<List<HistoryPostDocument>> GetFirstBatchLowerEqualThanIndex(int? index, int batchSize, string searchText = null);

        public Task InsertHistoryPost(string email, string title, string description, Image image);

        public Task InsertHistoryPost(UserDocument user, string title, string description, Image image);

        public Task DeleteHistoryPost(UserDocument user, int index);
    }
}
