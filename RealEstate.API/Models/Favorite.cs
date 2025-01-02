using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RealEstate.API.Models
{
    /// <summary>
    /// Represents a favorite property relationship between a user and a property
    /// </summary>
    public class Favorite
    {
        /// <summary>
        /// Gets or sets the unique identifier for the favorite relationship
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the ID of the user who favorited the property
        /// </summary>
        [BsonElement("userId")]
        [BsonRequired]
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the ID of the favorited property
        /// </summary>
        [BsonElement("propertyId")]
        [BsonRequired]
        public string PropertyId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date when the property was favorited
        /// </summary>
        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("property")]
        public Property? Property { get; set; }
    }
}
