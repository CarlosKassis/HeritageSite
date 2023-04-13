

namespace Miilya2023.Services.Concrete
{
    using Miilya2023.Services.Abstract;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;
    using MongoDB.Driver;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class ImageService : IImageService
    {
        private static readonly IMongoClient _mongoClient = new MongoClient("mongodb://localhost:27017");
        private static readonly IMongoDatabase _database = _mongoClient.GetDatabase("Miilya");
        private static readonly IMongoCollection<ImageDocument> _collection = _database.GetCollection<ImageDocument>("Images");

        public ImageService()
        {
            //var docs = Enumerable.Range(0, 1000).Select(i => new ImageDocument { Index = i });
            //_collection.InsertMany(docs);
        }

        public async Task<List<ImageDocument>> GetFirstBatchGreaterThanIndex(int index, int batchSize = 20)
        {
            var filter = Builders<ImageDocument>.Filter.Gte(x => x.Index, index);
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


    [BsonIgnoreExtraElements]
    public class ImageDocument
    {
        public string Name { get; set; }

        public int Index { get; set; }
    }
}
