
namespace HeritageSite.Services.Concrete
{
    using DnsClient.Protocol;
    using HeritageSite.Constants;
    using HeritageSite.Services.Abstract;
    using Microsoft.AspNetCore.Http;
    using MongoDB.Driver;
    using Neo4j.Driver;
    using Newtonsoft.Json;
    using SixLabors.ImageSharp;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using static HeritageSite.Services.Utils.Documents;

    public class HistoryPostServiceNeo : IHistoryPostService
    {
        private readonly IDriver _driver;

        public HistoryPostServiceNeo(IDriver driver)
        {
            _driver = driver;
        }

        public async Task DeleteHistoryPost(string userId, long index, bool isAdmin)
        {
            var record = await _driver.AsyncSession().ExecuteReadAsync(async tx =>
            {
                IResultCursor result;
                if (isAdmin)
                {
                    result = await tx.RunAsync(
$@"MATCH (post:HistoryPost {{ Index: {index}}})
return post");
                }
                else
                {
                    result = await tx.RunAsync(
$@"MATCH (post:HistoryPost {{ Index: {index}}})<-[b:POSTED]-(user: User {{ Id: '{userId}'}})
return post");
                }

                return (await result.ToListAsync())?.FirstOrDefault() ?? throw new InvalidOperationException("Post doesn't exist");
            });

            var imageName = record["post"].As<INode>()["ImageName"] as string;

            // Delete history post document
            await _driver.AsyncSession().ExecuteWriteAsync(async tx =>
            {
                if (isAdmin)
                {
                     await tx.RunAsync(
$@"MATCH (post:HistoryPost {{ Index: {index}}})
DETACH DELETE post");
                }
                else
                {
                    await tx.RunAsync(
$@"MATCH (post:HistoryPost {{ Index: {index}}})<-[:POSTED]-(user: User {{ Id: '{userId}'}})
DETACH DELETE post");
                }
            });

            // Beyond this point there's no return, since post document was deleted from DB
            // Failures here will be considered OK to user, but should be cleaned up by daemons
            try
            {
                // Delete image file
                if (!IsSecureImageFilename(imageName))
                {
                    return;
                }

                // TODO: add lock once daemons that clean up files get added
                await Task.Run(() => File.Delete(Path.Combine(PrivateHistoryConstants.ImagesDirectoryPath, imageName)));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public async Task<List<HistoryPostDocument>> GetFirstBatchLowerEqualThanIndex(int? index, int batchSize, string searchText = null)
        {
            var records = await _driver.AsyncSession().ExecuteReadAsync(async tx =>
            {
                string query = $@"
MATCH (post:HistoryPost)<-[:POSTED]-(user: User)
WHERE COALESCE(post.Index, 0) <= {index ?? int.MaxValue}
RETURN post, user
ORDER BY COALESCE(post.Index, 0)
DESC LIMIT {batchSize}";

                var results = await tx.RunAsync(query);

                return await results.ToListAsync();
            });

            return records
                .Select(x => {
                    var post = JsonConvert.DeserializeObject<HistoryPostDocument>(JsonConvert.SerializeObject(x["post"].As<INode>().Properties));
                    var posterId = x["user"].As<INode>()["Id"] as string;
                    post.PosterId = posterId;
                    return post;
                })
                .ToList();
        }

        public async Task InsertHistoryPost(string userId, string title, string description, Image image)
        {
            await CreateGraphUserIfDoesntExist(userId);
            string generatedFileName = GenerateUniqueFilename();

            try
            {
                Directory.CreateDirectory(PrivateHistoryConstants.ImagesDirectoryPath);
                using FileStream outputFile = new(Path.Combine(PrivateHistoryConstants.ImagesDirectoryPath, generatedFileName), FileMode.OpenOrCreate);
                await image.SaveAsJpegAsync(outputFile);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw new Exception($"Unable to save image");
            }

            await _driver.AsyncSession().ExecuteWriteAsync(async tx =>
            {
                string postId = Guid.NewGuid().ToString();
                await tx.RunAsync(
$@"
MERGE (maxIndex:MaxIndex)
ON CREATE
    SET maxIndex.Index = 0
with maxIndex
call apoc.atomic.add(maxIndex, 'Index', 1) yield newValue AS newIndex
MATCH (user: User {{ Id: '{userId}'}})
CREATE (post:HistoryPost)<-[:POSTED]-(user)
SET post.Id = '{postId}'
SET post.Index = newIndex
SET post.Title = '{title}'
SET post.Description = '{description}'
SET post.ImageName = '{generatedFileName}'
SET post.CreatedOn = {DateTimeOffset.UtcNow.ToUnixTimeSeconds()}");
            });
        }

        public async Task<IEnumerable<long>> GetUserBookmarkPostIndexes(string userId)
        {
            var records = await _driver.AsyncSession().ExecuteReadAsync(async tx =>
            {
                var results = await tx.RunAsync(
$@"MATCH (post:HistoryPost)<-[bookmarked:BOOKMARKED]-(user: User {{ Id: '{userId}'}})
RETURN post");

                return await results.ToListAsync();
            });

            return records
                .Select(x => x["post"].As<INode>()["Index"] as long?)
                .Cast<long>()
                .ToList();
        }

        public async Task AddBookmark(string userId, int historyPostIndex)
        {
            await _driver.AsyncSession().ExecuteWriteAsync(async tx =>
            {
                await tx.RunAsync(
                    $@"
                    MATCH (post:HistoryPost {{ Index: {historyPostIndex}}}), (user: User {{ Id: '{userId}'}})
                    CREATE (post)<-[b:BOOKMARKED]-(user)
                    ");
            });
        }

        public async Task RemoveBookmark(string userId, int historyPostIndex)
        {
            await _driver.AsyncSession().ExecuteWriteAsync(async tx =>
            {
                await tx.RunAsync(
                    $@"
                    MATCH (post:HistoryPost {{ Index: {historyPostIndex}}})<-[bookmark:BOOKMARKED]-(user: User {{ Id: '{userId}'}})
                    DELETE bookmark
                    ");
            });
        }

        public async Task CreateGraphUserIfDoesntExist(string userId)
        {
            await _driver.AsyncSession().ExecuteWriteAsync(async tx =>
            {
                await tx.RunAsync($"MERGE (:User{{ Id: '{userId}'}})");
            });
        }

        private static string GenerateUniqueFilename()
        {
            return $"{DateTimeOffset.UtcNow:yyyyMMdd-HHmmss}-{RandomNumericString()}.jpg";
        }

        private static string RandomNumericString(int length = 6)
        {
            var random = new Random();
            const string chars = "0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private static bool IsSecureImageFilename(string filename)
        {
            if (string.IsNullOrEmpty(filename))
            {
                return false;
            }

            var parts = filename.Split('.');

            if (parts.Length != 2)
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
