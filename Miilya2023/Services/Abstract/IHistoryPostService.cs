
namespace Miilya2023.Services.Abstract
{
    using SixLabors.ImageSharp;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using static Miilya2023.Services.Utils.Documents;

    public interface IHistoryPostService
    {
        public Task<List<HistoryPostDocument>> GetFirstBatchLowerEqualThanIndex(int? index, int batchSize);

        public Task InsertHistoryPost(string title, string description, Image image);
    }
}
