
namespace HeritageSite.Services.Utils
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;
    using Newtonsoft.Json;

    public class Documents
    {
        /// <summary>
        /// Describes a family tree.
        /// </summary>
        [BsonIgnoreExtraElements]
        public class FamilyDocument
        {
            [BsonElement("_id")]
            public ObjectId Id { get; set; }

            /// <summary>
            /// Display name of the family.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Used to search family tree files on server.
            /// </summary>
            public string Identifier { get; set; }


            /// <summary>
            /// Indicates whether there are files or not.
            /// </summary>
            public bool Available { get; set; }
        }

        public class HistoryPostDocument
        {
            public string Id { get; set; }

            [JsonIgnore]
            public string PosterId { get; set; }

            public string ImageName { get; set; }

            public long CreatedOn { get; set; }

            public string ImageDate { get; set; }

            public string Title { get; set; }

            public string Description { get; set; }

            public long Index { get; set; }
        }

        public class UserDocument
        {
            public string Id { get; set; }

            public string Email { get; set; }

            public bool IsAdmin { get; set; }
        }

        public class BookmarkDocument
        {
            public string Id { get; set; }

            public string UserId { get; set; }

            public int HistoryPostIndex { get; set; }
        }
    }
}
