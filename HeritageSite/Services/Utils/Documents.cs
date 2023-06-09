﻿
namespace HeritageSite.Services.Utils
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;
    using System.Collections.Generic;

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

        [BsonIgnoreExtraElements]
        public class HistoryPostDocument
        {
            [BsonElement("_id")]
            public ObjectId Id { get; set; }

            public ObjectId UserId { get; set; }

            public string ImageName { get; set; }

            public BsonDateTime ImageDate { get; set; }

            public string Title { get; set; }

            public string Description { get; set; }

            public int Index { get; set; }
        }

        [BsonIgnoreExtraElements]
        public class UserDocument
        {
            [BsonElement("_id")]
            public ObjectId Id { get; set; }

            public string EmailSHA256 { get; set; }

            public bool IsAdmin { get; set; }
        }

        [BsonIgnoreExtraElements]
        public class BookmarkDocument
        {
            [BsonElement("_id")]
            public ObjectId Id { get; set; }

            public ObjectId UserId { get; set; }

            public int HistoryPostIndex { get; set; }
        }
    }
}
