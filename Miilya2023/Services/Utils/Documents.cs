
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
            /// Display name in Arabic of the family
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Used to search family tree files on server
            /// </summary>
            public string Identifier { get; set; }
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
