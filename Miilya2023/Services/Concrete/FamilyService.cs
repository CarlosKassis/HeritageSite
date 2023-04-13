

namespace Miilya2023.Services.Concrete
{
    using Miilya2023.Services.Abstract;
    using MongoDB.Bson.Serialization.Attributes;
    using MongoDB.Driver;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class FamilyService : IFamilyService
    {
        private static readonly IMongoClient _mongoClient = new MongoClient("mongodb://localhost:27017");
        private static readonly IMongoDatabase _database = _mongoClient.GetDatabase("Miilya");
        private static readonly IMongoCollection<FamilyDocument> _collection = _database.GetCollection<FamilyDocument>("Families");

        public FamilyService()
        {
        }

        public async Task<List<FamilyDocument>> GetAll()
        {
            var results = await _collection
                .Find(_ => true)
                .ToListAsync();

            return results;
        }
    }


    [BsonIgnoreExtraElements]
    public class FamilyDocument
    {
        public string Name { get; set; }

        public string Identifier { get; set; }
    }
}
