using System;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace RealEstate.API.Models
{
    /// <summary>
    /// Represents a real estate property listing
    /// </summary>
    public class Property
    {
        /// <summary>
        /// Gets or sets the unique identifier for the property
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the ID of the user who listed the property
        /// </summary>
        [BsonElement("userId")]
        [JsonPropertyName("userId")]
        [Required]
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the title of the property listing
        /// </summary>
        [BsonElement("title")]
        [JsonPropertyName("title")]
        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the detailed description of the property
        /// </summary>
        [BsonElement("description")]
        [JsonPropertyName("description")]
        [Required]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the price of the property
        /// </summary>
        [BsonElement("price")]
        [JsonPropertyName("price")]
        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        /// <summary>
        /// Gets or sets the location of the property
        /// </summary>
        [BsonElement("location")]
        [JsonPropertyName("location")]
        [Required]
        public string Location { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the type of property (e.g., Apartment, House)
        /// </summary>
        [BsonElement("type")]
        [JsonPropertyName("type")]
        [Required]
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the status of the property (e.g., For Sale, For Rent)
        /// </summary>
        [BsonElement("isForRent")]
        [JsonPropertyName("isForRent")]
        [Required]
        public bool IsForRent { get; set; }

        /// <summary>
        /// Gets or sets the number of bedrooms in the property
        /// </summary>
        [BsonElement("bedrooms")]
        [JsonPropertyName("bedrooms")]
        [Required]
        [Range(0, 20)]
        public int Bedrooms { get; set; }

        /// <summary>
        /// Gets or sets the number of bathrooms in the property
        /// </summary>
        [BsonElement("bathrooms")]
        [JsonPropertyName("bathrooms")]
        [Required]
        [Range(0, 20)]
        public int Bathrooms { get; set; }

        /// <summary>
        /// Gets or sets the size of the property in square meters
        /// </summary>
        [BsonElement("size")]
        [JsonPropertyName("size")]
        [Required]
        public double Size { get; set; }

        /// <summary>
        /// Gets or sets the list of features/amenities of the property
        /// </summary>
        [BsonElement("features")]
        [JsonPropertyName("features")]
        public List<string> Features { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the main image URL of the property
        /// </summary>
        [BsonElement("imageUrl")]
        [JsonPropertyName("imageUrl")]
        public string ImageUrl { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets additional image URLs of the property
        /// </summary>
        [BsonElement("imageUrls")]
        [JsonPropertyName("imageUrls")]
        [Required]
        public System.Collections.Generic.List<string> ImageUrls { get; set; } = new System.Collections.Generic.List<string>();

        /// <summary>
        /// Gets or sets the date when the property was listed
        /// </summary>
        [BsonElement("createdAt")]
        [JsonPropertyName("createdAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the date when the property was last updated
        /// </summary>
        [BsonElement("updatedAt")]
        [JsonPropertyName("updatedAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Gets or sets the address of the property
        /// </summary>
        [BsonElement("address")]
        [JsonPropertyName("address")]
        [Required]
        [StringLength(100)]
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the city of the property
        /// </summary>
        [BsonElement("city")]
        [JsonPropertyName("city")]
        [Required]
        [StringLength(100)]
        public string City { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the state of the property
        /// </summary>
        [BsonElement("state")]
        [JsonPropertyName("state")]
        [Required]
        [StringLength(100)]
        public string State { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the zip code of the property
        /// </summary>
        [BsonElement("zipCode")]
        [JsonPropertyName("zipCode")]
        [Required]
        [StringLength(100)]
        public string ZipCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the square meters of the property
        /// </summary>
        [BsonElement("squareMeters")]
        [JsonPropertyName("squareMeters")]
        [Required]
        [Range(0, double.MaxValue)]
        public double SquareMeters { get; set; }

        /// <summary>
        /// Gets or sets the property type of the property
        /// </summary>
        [BsonElement("propertyType")]
        [JsonPropertyName("propertyType")]
        [Required]
        [StringLength(50)]
        public string PropertyType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets additional images of the property
        /// </summary>
        [BsonElement("images")]
        [JsonPropertyName("images")]
        public List<string> Images { get; set; } = new List<string>();
    }
}
