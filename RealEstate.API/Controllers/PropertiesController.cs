using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using RealEstate.API.Models;
using System.IO;

namespace RealEstate.API.Controllers
{
    /// <summary>
    /// Controller for managing property listings
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PropertiesController : ControllerBase
    {
        private readonly IMongoCollection<Property> _properties;
        private readonly IMongoCollection<User> _users;

        public PropertiesController(IMongoDatabase database)
        {
            _properties = database.GetCollection<Property>("Properties");
            _users = database.GetCollection<User>("Users");
        }

        /// <summary>
        /// Gets all properties with optional filtering
        /// </summary>
        /// <param name="city">City filter</param>
        /// <param name="propertyType">Property type filter</param>
        /// <param name="minPrice">Minimum price filter</param>
        /// <param name="maxPrice">Maximum price filter</param>
        /// <returns>List of all properties</returns>
        /// <response code="200">Returns the list of properties</response>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Property>>> GetAllProperties(
            [FromQuery] string? city = null, 
            [FromQuery] string? propertyType = null, 
            [FromQuery] decimal? minPrice = null, 
            [FromQuery] decimal? maxPrice = null)
        {
            var filter = Builders<Property>.Filter.Empty;

            if (!string.IsNullOrEmpty(city))
                filter &= Builders<Property>.Filter.Eq(p => p.City, city);

            if (!string.IsNullOrEmpty(propertyType))
                filter &= Builders<Property>.Filter.Eq(p => p.PropertyType, propertyType);

            if (minPrice.HasValue)
                filter &= Builders<Property>.Filter.Gte(p => p.Price, minPrice.Value);

            if (maxPrice.HasValue)
                filter &= Builders<Property>.Filter.Lte(p => p.Price, maxPrice.Value);

            var properties = await _properties.Find(filter).ToListAsync();
            return Ok(properties);
        }

        /// <summary>
        /// Gets a specific property by ID
        /// </summary>
        /// <param name="id">Property ID</param>
        /// <returns>Property details</returns>
        /// <response code="200">Returns the property details</response>
        /// <response code="404">If the property is not found</response>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Property>> GetProperty(string id)
        {
            var property = await _properties.Find(p => p.Id == id).FirstOrDefaultAsync();
            
            if (property == null)
                return NotFound();

            return Ok(property);
        }

        /// <summary>
        /// Gets all properties belonging to the authenticated user
        /// </summary>
        /// <returns>List of user's properties</returns>
        /// <response code="200">Returns the list of user's properties</response>
        /// <response code="401">If the user is not authenticated</response>
        [HttpGet("user")]
        public async Task<ActionResult<IEnumerable<Property>>> GetUserProperties()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            
            if (userId == null)
                return Unauthorized();

            var properties = await _properties.Find(p => p.UserId == userId).ToListAsync();
            return Ok(properties);
        }

        /// <summary>
        /// Creates a new property listing
        /// </summary>
        /// <param name="property">Property details</param>
        /// <returns>Created property details</returns>
        /// <response code="201">Returns the created property</response>
        /// <response code="400">If the property data is invalid</response>
        [HttpPost]
        public async Task<ActionResult<Property>> CreateProperty([FromBody] Property property)
        {
            try 
            {
                if (!ModelState.IsValid)
                {
                    Console.WriteLine("Model validation failed:");
                    foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        Console.WriteLine($"- {error.ErrorMessage}");
                    }
                    return BadRequest(ModelState);
                }

                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                
                if (string.IsNullOrEmpty(userId))
                {
                    Console.WriteLine("Unauthorized: No user ID found in token");
                    return Unauthorized();
                }

                var user = await _users.Find(u => u.Id == userId).FirstOrDefaultAsync();
                
                if (user == null)
                {
                    Console.WriteLine($"User not found with ID: {userId}");
                    return Unauthorized();
                }

                // Set the user-related fields
                property.UserId = userId;
                property.CreatedAt = DateTime.UtcNow;

                Console.WriteLine($"Creating property for user: {userId}");
                Console.WriteLine($"Property data: {System.Text.Json.JsonSerializer.Serialize(property)}");

                await _properties.InsertOneAsync(property);

                // Add the property to the user's properties list
                if (user.Properties == null)
                {
                    user.Properties = new List<Property>();
                }
                user.Properties.Add(property);
                await _users.ReplaceOneAsync(u => u.Id == userId, user);

                Console.WriteLine($"Property created successfully with ID: {property.Id}");
                return CreatedAtAction(nameof(GetProperty), new { id = property.Id }, property);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating property: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        /// <summary>
        /// Updates an existing property
        /// </summary>
        /// <param name="id">Property ID</param>
        /// <param name="updatedProperty">Updated property details</param>
        /// <returns>Updated property details</returns>
        /// <response code="200">Returns the updated property</response>
        /// <response code="404">If the property is not found</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProperty(string id, [FromBody] Property updatedProperty)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            
            if (userId == null)
                return Unauthorized();

            var existingProperty = await _properties.Find(p => p.Id == id && p.UserId == userId).FirstOrDefaultAsync();
            
            if (existingProperty == null)
                return NotFound();

            updatedProperty.Id = id;
            updatedProperty.UserId = userId;
            updatedProperty.CreatedAt = existingProperty.CreatedAt;
            updatedProperty.UpdatedAt = DateTime.UtcNow;

            await _properties.ReplaceOneAsync(p => p.Id == id, updatedProperty);

            return NoContent();
        }

        /// <summary>
        /// Deletes a property
        /// </summary>
        /// <param name="id">Property ID</param>
        /// <returns>No content if successful</returns>
        /// <response code="204">Property deleted successfully</response>
        /// <response code="404">If the property is not found</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProperty(string id)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            
            if (userId == null)
                return Unauthorized();

            var property = await _properties.Find(p => p.Id == id).FirstOrDefaultAsync();
            
            if (property == null)
                return NotFound();

            if (property.UserId != userId)
                return Forbid();

            await _properties.DeleteOneAsync(p => p.Id == id);
            return NoContent();
        }

        /// <summary>
        /// Uploads images for a property
        /// </summary>
        /// <param name="id">Property ID</param>
        /// <param name="images">Images to upload</param>
        /// <returns>Updated property details</returns>
        /// <response code="200">Returns the updated property</response>
        /// <response code="400">If the images are invalid</response>
        [HttpPost("{id}/images")]
        public async Task<ActionResult<Property>> UploadImages(string id, [FromForm] IFormFileCollection images)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var property = await _properties.Find(p => p.Id == id && p.UserId == userId).FirstOrDefaultAsync();
                if (property == null)
                {
                    return NotFound();
                }

                if (images == null || images.Count == 0)
                {
                    return BadRequest("No images uploaded");
                }

                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "properties", id);
                Directory.CreateDirectory(uploadPath);

                var imageUrls = new List<string>();
                foreach (var image in images)
                {
                    if (image.Length > 0)
                    {
                        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
                        var filePath = Path.Combine(uploadPath, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await image.CopyToAsync(stream);
                        }

                        var imageUrl = $"/uploads/properties/{id}/{fileName}";
                        imageUrls.Add(imageUrl);
                    }
                }

                // Update property with image URLs
                property.ImageUrls.AddRange(imageUrls);
                if (!string.IsNullOrEmpty(imageUrls.FirstOrDefault()))
                {
                    property.ImageUrl = imageUrls.First();
                }
                property.UpdatedAt = DateTime.UtcNow;

                await _properties.ReplaceOneAsync(p => p.Id == id, property);

                return Ok(property);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading images: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return StatusCode(500, "Error uploading images");
            }
        }
    }
}
