using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.API.Models
{
    /// <summary>
    /// Represents a user in the system
    /// </summary>
    public class User
    {
        /// <summary>
        /// Gets or sets the unique identifier for the user
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user's email address
        /// Used for authentication and communication
        /// </summary>
        [BsonElement("email")]
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user's password hash
        /// Stored as a secure hash, never as plain text
        /// </summary>
        [BsonElement("passwordHash")]
        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user's first name
        /// </summary>
        [BsonElement("firstName")]
        [Required]
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user's last name
        /// </summary>
        [BsonElement("lastName")]
        [Required]
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user's phone number
        /// </summary>
        [BsonElement("phoneNumber")]
        [Required]
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date when the user account was created
        /// </summary>
        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the date when the user account was last updated
        /// </summary>
        [BsonElement("updatedAt")]
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Gets or sets the properties owned by the user
        /// </summary>
        [BsonElement("properties")]
        public List<Property> Properties { get; set; } = new List<Property>();
    }

    public class UserDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
    }

    public class LoginDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
