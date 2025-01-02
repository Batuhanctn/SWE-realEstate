using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using RealEstate.API.Data;
using RealEstate.API.Models;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace RealEstate.API.Controllers
{
    /// <summary>
    /// Controller for managing user's favorite properties
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class FavoriteController : ControllerBase
    {
        private readonly IMongoCollection<Favorite> _favorites;
        private readonly IMongoCollection<Property> _properties;
        private readonly ILogger<FavoriteController> _logger;

        public FavoriteController(RealEstateDbContext context, ILogger<FavoriteController> logger)
        {
            _favorites = context.Favorites;
            _properties = context.Properties;
            _logger = logger;
        }

        /// <summary>
        /// Gets all favorite properties for the authenticated user
        /// </summary>
        /// <returns>List of favorite properties</returns>
        /// <response code="200">Returns the list of favorite properties</response>
        /// <response code="401">If the user is not authenticated</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Property>>> GetFavorites()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            var favoritesCursor = await _favorites.FindAsync(f => f.UserId == userId);
            var favorites = await favoritesCursor.ToListAsync();

            var propertyIds = favorites.Select(f => f.PropertyId).ToList();
            var propertiesCursor = await _properties.FindAsync(p => propertyIds.Contains(p.Id));
            var properties = await propertiesCursor.ToListAsync();

            return Ok(properties);
        }

        public class AddFavoriteRequest
        {
            public string propertyId { get; set; } = string.Empty;
        }

        /// <summary>
        /// Adds a property to user's favorites
        /// </summary>
        /// <param name="request">Request object containing the property ID to add to favorites</param>
        /// <returns>Success message if property is added to favorites</returns>
        /// <response code="200">Property added to favorites successfully</response>
        /// <response code="400">If the property is already in favorites</response>
        /// <response code="404">If the property is not found</response>
        [HttpPost]
        public async Task<ActionResult<Favorite>> AddToFavorites([FromBody] AddFavoriteRequest request)
        {
            _logger.LogInformation($"Received request: {JsonSerializer.Serialize(request)}");

            if (request == null)
            {
                _logger.LogWarning("Request is null");
                return BadRequest("Request is null");
            }

            if (string.IsNullOrEmpty(request.propertyId))
            {
                _logger.LogWarning("PropertyId is empty");
                return BadRequest("PropertyId is required");
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            _logger.LogInformation($"Looking for property with ID: {request.propertyId}");
            var propertyCursor = await _properties.FindAsync(p => p.Id == request.propertyId);
            var property = await propertyCursor.FirstOrDefaultAsync();

            if (property == null)
            {
                _logger.LogWarning($"Property not found with ID: {request.propertyId}");
                return NotFound("Property not found");
            }

            var favoriteCursor = await _favorites.FindAsync(f => f.UserId == userId && f.PropertyId == request.propertyId);
            var existingFavorite = await favoriteCursor.FirstOrDefaultAsync();

            if (existingFavorite != null)
            {
                return BadRequest("Property is already in favorites");
            }

            var favorite = new Favorite
            {
                UserId = userId,
                PropertyId = request.propertyId,
                CreatedAt = DateTime.UtcNow
            };

            await _favorites.InsertOneAsync(favorite);
            return Ok(new { id = favorite.Id, propertyId = favorite.PropertyId });
        }

        /// <summary>
        /// Removes a property from user's favorites
        /// </summary>
        /// <param name="propertyId">ID of the property to remove from favorites</param>
        /// <returns>Success message if property is removed from favorites</returns>
        /// <response code="200">Property removed from favorites successfully</response>
        /// <response code="404">If the property is not found in favorites</response>
        [HttpDelete("{propertyId}")]
        public async Task<IActionResult> RemoveFromFavorites(string propertyId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            var result = await _favorites.DeleteOneAsync(
                f => f.UserId == userId && f.PropertyId == propertyId);

            return NoContent();
        }

        /// <summary>
        /// Checks if a property is in user's favorites
        /// </summary>
        /// <param name="propertyId">ID of the property to check</param>
        /// <returns>True if property is in favorites, false otherwise</returns>
        /// <response code="200">Returns whether the property is in favorites</response>
        /// <response code="404">If the property is not found</response>
        [HttpGet("{propertyId}")]
        public async Task<ActionResult<bool>> CheckIsFavorite(string propertyId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            var cursor = await _favorites.FindAsync(f => f.UserId == userId && f.PropertyId == propertyId);
            var exists = await cursor.MoveNextAsync();
            var hasItems = exists && cursor.Current.Any();

            return Ok(hasItems);
        }
    }
}
