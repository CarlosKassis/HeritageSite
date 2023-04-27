

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
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using static Miilya2023.Services.Utils.Documents;

    public class HistoryPostService : IHistoryPostService
    {
        private static readonly IMongoClient _mongoClient = new MongoClient("mongodb://localhost:27017");
        private static readonly IMongoDatabase _database = _mongoClient.GetDatabase(PrivateHistoryConstants.DatabaseName);
        private static readonly IMongoCollection<HistoryPostDocument> _collection = _database.GetCollection<HistoryPostDocument>("HistoryPosts");
        private static readonly Random _random = new ();

        private readonly IUserService _userService;
        private readonly IBookmarkService _bookmarkService;

        public HistoryPostService(IUserService userService, IBookmarkService bookmarkService)
        {
            _userService = userService;
            _bookmarkService = bookmarkService;
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

        public async Task InsertHistoryPost(string email, string title, string description, Image image)
        {
            var user = await _userService.GetUserWithEmail(email);
            await InsertHistoryPost(user, title, description, image);
        }

        public async Task InsertHistoryPost(UserDocument user, string title, string description, Image image)
        {
            string generatedFileName = GenerateUniqueFilename();

            try
            {
                using FileStream outputFile = new(Path.Combine(PrivateHistoryConstants.RootPath, "Media", "Images", generatedFileName), FileMode.OpenOrCreate);
                await image.SaveAsJpegAsync(outputFile);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw new Exception($"Unable to save image");
            }

            var historyPost = new HistoryPostDocument
            {
                UserId = user.Id,
                Title = title,
                Description = description,
                ImageName = generatedFileName,
                Index = GetNewMaxDocumentIndexInDb()
            };

            _collection.InsertOne(historyPost);
        }


        public async Task DeleteHistoryPost(UserDocument user, int index)
        {
            // Delete history post document
            var filter = Builders<HistoryPostDocument>.Filter.Eq(x => x.Index, index);
            if (!user.IsAdmin)
            {
                filter &= Builders<HistoryPostDocument>.Filter.Eq(x => x.UserId, user.Id);
            }

            var historyPost = await _collection.Find(filter).FirstAsync();
            if (historyPost == null)
            {
                throw new InvalidOperationException("Invalid delete");
            }

            var deleteResult = await _collection.DeleteOneAsync(filter);
            if (deleteResult.DeletedCount == 0)
            {
                throw new InvalidOperationException("Unable to delete to post");
            }

            // Beyond this point there's no return, since post document was deleted from DB
            // Failures here will be considered OK to user, but should be cleaned up by daemons

            // Delete bookmarks for this post
            await _bookmarkService.DeleteBookmarksForHistoryPost(historyPost.Index);

            try
            {
                // Delete image file
                if (!IsSecureImageFilename(historyPost.ImageName))
                {
                    return;
                }

                // TODO: add lock once daemons that clean up files get added
                await Task.Run(() => File.Delete(Path.Combine(PrivateHistoryConstants.RootPath, "Media", "Images", historyPost.ImageName)));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
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

        private static bool IsSecureImageFilename(string filename)
        {
            if (string.IsNullOrEmpty(filename))
            {
                return false;
            }

            var parts = filename.Split('.');

            if (parts.Length != 2 )
            {
                return false;
            }

            if (parts.Last() != "jpg")
            {
                return false;
            }

            if (parts.Length > 16)
            {
                return false;
            }

            if (parts.First().Any(c => !char.IsNumber(c) && c != '-'))
            {
                return false;
            }

            return true;
        }
    }
}
