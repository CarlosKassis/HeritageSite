

namespace Miilya2023.Services.Concrete
{
    using Miilya2023.Services.Abstract;
    using MongoDB.Driver;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using static Miilya2023.Services.Utils.Documents;

    public class HistoryPostService : IHistoryPostService
    {
        private static readonly IMongoClient _mongoClient = new MongoClient("mongodb://localhost:27017");
        private static readonly IMongoDatabase _database = _mongoClient.GetDatabase("Miilya");
        private static readonly IMongoCollection<HistoryPostDocument> _collection = _database.GetCollection<HistoryPostDocument>("HistoryPosts");

        public HistoryPostService()
        {
            //var docs = Enumerable.Range(0, 1000).Select(i => new ImageDocument { Index = i });
            //_collection.InsertMany(docs);
        }

        public async Task<List<HistoryPostDocument>> GetFirstBatchGreaterEqualThanIndex(int index, int batchSize = 20)
        {
            var filter = Builders<HistoryPostDocument>.Filter.Gte(x => x.Index, index);
            var results = await _collection
                .Find(filter)
                .Limit(batchSize)
                .ToListAsync();

            return results;
        }

        private int GetMaxDocumentIndexInDb()
        {
            //var maxIndexDocument = _collection.Find(x => true).SortByDescending(x => x[nameof(ImageDocument.Index)]).FirstOrDefault();
            return 1; // maxIndexDocument == null ? 0 : BsonSerializer.Deserialize<ImageDocument>(maxIndexDocument).Index;
        }
    }
}
