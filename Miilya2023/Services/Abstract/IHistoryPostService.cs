
namespace Miilya2023.Services.Abstract
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using static Miilya2023.Services.Utils.Documents;

    public interface IHistoryPostService
    {
        public Task<List<HistoryPostDocument>> GetFirstBatchGreaterEqualThanIndex(int index, int batchSize = 20);
    }
}
