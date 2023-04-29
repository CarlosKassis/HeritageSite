

namespace Miilya2023.Services.Concrete
{
    using CsvHelper;
    using Microsoft.Extensions.Logging;
    using Miilya2023.Constants;
    using Miilya2023.Services.Abstract;
    using MongoDB.Driver;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using static Miilya2023.Controllers.PrivateHistory.FamilyController;
    using static Miilya2023.Services.Utils.Documents;

    public class FamilyService : IFamilyService
    {
        private static readonly IMongoClient _mongoClient = new MongoClient("mongodb://localhost:27017");
        private static readonly IMongoDatabase _database = _mongoClient.GetDatabase(PrivateHistoryConstants.DatabaseName);
        private static readonly IMongoCollection<FamilyDocument> _collection = _database.GetCollection<FamilyDocument>("Families");

        private static Task _familyCsvReaderDaemon;
        private static object _familyTreesLock = new();
        private static ConcurrentDictionary<string, string> _familyTreesSerialized = null;

        private ILogger<FamilyService> _logger;

        public FamilyService(ILogger<FamilyService> logger)
        {
            _logger = logger;

            bool needToReadCsvs = false;
            lock (_familyTreesLock)
            {
                if (_familyTreesSerialized == null)
                {
                    _familyTreesSerialized = new();
                    needToReadCsvs = true;
                }
            }

            if (needToReadCsvs)
            {
                ReadAllFamilyTreeCsvs(_logger);
            }

            _familyCsvReaderDaemon ??= Task.Factory.StartNew(async () => await ReadFamilyTreeCsvsLoop(logger));
        }

        private static async Task ReadFamilyTreeCsvsLoop(ILogger<FamilyService> logger)
        {
            while (true)
            {
                try
                {
                    ReadAllFamilyTreeCsvs(logger);
                }
                catch (Exception ex)
                {
                    logger?.LogError(ex.Message, ex);
                    await Task.Delay(TimeSpan.FromMinutes(1));
                }
            }
        }

        public async Task<List<FamilyDocument>> GetAll()
        {
            var results = await _collection
                .Find(_ => true)
                .ToListAsync();

            return results;
        }

        public string GetFamilyTreeSerialized(string familyId)
        {
            if (!_familyTreesSerialized.TryGetValue(familyId, out var familyTreeSerialized))
            {
                throw new InvalidOperationException("Couldn't load family tree info");
            }


            return familyTreeSerialized;
        }

        private static void ReadAllFamilyTreeCsvs(ILogger<FamilyService> logger)
        {
            // TODO: don't update if file didn't change
            // TODO: add assertions against DB:
            // Add and remove from dictionary to align readable .CSVs with available families in DB
            foreach (var family in Directory.EnumerateDirectories(PrivateHistoryConstants.FamiliesDirectoryPath))
            {
                try
                {
                    var familyId = new DirectoryInfo(family).Name;
                    List<FamilyEchoCsvEntry> records = null;
                    var csvPath = Path.Combine(family, $"{familyId}.csv");
                    using (var reader = new StreamReader(csvPath, new FileStreamOptions { Access = FileAccess.Read }))
                    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {
                        records = csv.GetRecords<FamilyEchoCsvEntry>().ToList();
                    }

                    var familyTreePeople = records.Select(record => record.ToBalkanFamilyTreePerson());
                    _familyTreesSerialized[familyId] = JsonConvert.SerializeObject(familyTreePeople);
                }
                catch (Exception ex)
                {
                    logger?.LogError(ex.Message, ex);
                }
            }
        }
    }
}
