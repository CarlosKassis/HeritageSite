

namespace Miilya2023.Services.Concrete
{
    using Microsoft.AspNetCore.Http;
    using Miilya2023.Constants;
    using Miilya2023.Services.Abstract;
    using MongoDB.Driver;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection.Metadata;
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

        public async Task<List<HistoryPostDocument>> GetFirstBatchLowerEqualThanIndex(int? index, int batchSize)
        {
            index ??= GetNewMaxDocumentIndexInDb();
            var filter = Builders<HistoryPostDocument>.Filter.Lte(x => x.Index, index);
            var results = await _collection
                .Find(filter)
                .SortByDescending(x => x.Index)
                .Limit(batchSize)
                .ToListAsync();

            return results;
        }

        public async Task InsertHistoryPost(string title, string description, IFormFile imageFile)
        {
            // TODO: assert image
            if (imageFile != null)
            {
                try
                {
                    using FileStream outputFile = new FileStream(Path.Combine(PrivateHistoryConstants.RootPath, "Media", "Images", imageFile.FileName), FileMode.OpenOrCreate);
                    await imageFile.CopyToAsync(outputFile);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw new Exception($"Unable to save image: {imageFile.Name}");
                }
            }

            _collection.InsertOne(new HistoryPostDocument { Title = title, Description = description, ImageName = imageFile?.FileName, Index = GetNewMaxDocumentIndexInDb() });
        }

        private int GetNewMaxDocumentIndexInDb()
        {
            var maxIndexDocument = _collection.Find(x => true).SortByDescending(x => x.Index).FirstOrDefault()?.Index ?? 0;
            return maxIndexDocument + 1;
        }
    }
}
