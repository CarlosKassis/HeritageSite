
namespace HeritageSite.Services.Utils
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;
    using System;

    public class DocumentsExternal
    {
        /// <summary>
        /// Descripts a family tree
        /// </summary>
        [BsonIgnoreExtraElements]
        public class FamilyDocumentExternal
        {
            /// <summary>
            /// Display name of the family
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Used to search family tree files on server
            /// </summary>
            public string Identifier { get; set; }
        }

        [BsonIgnoreExtraElements]
        public class HistoryPostDocumentExternal
        {
            public string ImageName { get; set; }

            public BsonDateTime ImageDate { get; set; }

            public string Title { get; set; }

            public string Description { get; set; }

            public long Index { get; set; }

            public int Control { get; set; }

            public bool Bookmarked { get; set; }
        }

        [BsonIgnoreExtraElements]
        public class UserDocumentExternal
        {
            public string Id { get; set; }
        }
    }
}
