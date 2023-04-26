

namespace Miilya2023.Services.Concrete
{
    using Microsoft.AspNetCore.Http;
    using Miilya2023.Constants;
    using Miilya2023.Services.Abstract;
    using Miilya2023.Shared;
    using MongoDB.Driver;
    using SixLabors.ImageSharp;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using static Miilya2023.Services.Utils.Documents;

    public class HistoryPostService : IHistoryPostService
    {
        private static readonly IMongoClient _mongoClient = new MongoClient("mongodb://localhost:27017");
        private static readonly IMongoDatabase _database = _mongoClient.GetDatabase("Miilya");
        private static readonly IMongoCollection<HistoryPostDocument> _collection = _database.GetCollection<HistoryPostDocument>("HistoryPosts");

        private static readonly Random _random = new ();

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

        public async Task InsertHistoryPost(string title, string description, Image image)
        {
            string generatedFileName = GenerateUniqueFilename();

            try
            {
                using FileStream outputFile = new (Path.Combine(PrivateHistoryConstants.RootPath, "Media", "Images", generatedFileName), FileMode.OpenOrCreate);
                await image.SaveAsJpegAsync(outputFile);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw new Exception($"Unable to save image");
            }

            _collection.InsertOne(new HistoryPostDocument { Title = title, Description = description, ImageName = generatedFileName, Index = GetNewMaxDocumentIndexInDb() });
        }

        private static int GetNewMaxDocumentIndexInDb()
        {
            var maxIndexDocument = _collection.Find(x => true).SortByDescending(x => x.Index).FirstOrDefault()?.Index ?? 0;
            return maxIndexDocument + 1;
        }

        private static string GenerateUniqueFilename()
        {
            return $"{DateTimeOffset.UtcNow:yyyyMMdd-HHmmss}-{RandomNumericString()}.jpg";
        }

        private static string RandomNumericString(int length = 6)
        {
            const string chars = "0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[_random.Next(s.Length)]).ToArray());
        }
    }
}
