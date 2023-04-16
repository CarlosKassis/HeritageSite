
namespace Miilya2023.Services.Utils
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    public class Documents
    {
        /// <summary>
        /// Descripts a family tree
        /// </summary>
        [BsonIgnoreExtraElements]
        public class FamilyDocument
        {
            /// <summary>
            /// Display name of the family
            /// </summary>
            public string Name;

            /// <summary>
            /// Used to search family tree files on server
            /// </summary>
            public string Identifier;
        }

        [BsonIgnoreExtraElements]
        public class HistoryPostDocument
        {
            public string ImageName;

            public BsonDateTime ImageDate;

            public string Title;

            public string Description;

            public int Index;
        }
    }
}
