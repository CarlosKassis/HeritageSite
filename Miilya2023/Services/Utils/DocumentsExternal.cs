
namespace Miilya2023.Services.Utils
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

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

            public int Index { get; set; }

            public bool MyPost { get; set; }
        }

        [BsonIgnoreExtraElements]
        public class UserDocumentExternal
        {
            public string EmailSHA256 { get; set; }
        }
    }
}
