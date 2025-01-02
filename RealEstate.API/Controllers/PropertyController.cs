using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstate.API.Models;
using RealEstate.API.Services;

namespace RealEstate.API.Controllers
{
    /// <summary>
    /// Controller for managing real estate properties
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PropertyController : ControllerBase
    {
        private readonly IPropertyService _propertyService;
        private readonly ILogger<PropertyController> _logger;

        /// <summary>
        /// Initializes a new instance of the PropertyController
        /// </summary>
        /// <param name="propertyService">The property service for managing properties</param>
        /// <param name="logger">The logger for the controller</param>
        public PropertyController(IPropertyService propertyService, ILogger<PropertyController> logger)
        {
            _propertyService = propertyService;
            _logger = logger;
        }

        /// <summary>
        /// Gets all properties with optional filtering
        /// </summary>
        /// <returns>List of properties matching the criteria</returns>
        /// <response code="200">Returns the list of properties</response>
        /// <response code="401">If the user is not authenticated</response>
        [HttpGet]
        public async Task<ActionResult<List<Property>>> Get(
            [FromQuery] string? city = null,
            [FromQuery] string? propertyType = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null,
            [FromQuery] int? bedrooms = null,
            [FromQuery] int? bathrooms = null,
            [FromQuery] double? minSquareMeters = null,
            [FromQuery] double? maxSquareMeters = null)
        {
            var properties = await _propertyService.GetFilteredPropertiesAsync(
                city, propertyType, minPrice, maxPrice, 
                bedrooms, bathrooms, minSquareMeters, maxSquareMeters);
            return Ok(properties);
        }

        /// <summary>
        /// Gets a specific property by ID
        /// </summary>
        /// <param name="id">The ID of the property</param>
        /// <returns>The requested property</returns>
        /// <response code="200">Returns the requested property</response>
        /// <response code="404">If the property is not found</response>
        /// <response code="401">If the user is not authenticated</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<Property>> Get(string id)
        {
            var property = await _propertyService.GetAsync(id);
            if (property == null)
                return NotFound();

            return Ok(property);
        }

        /// <summary>
        /// Creates a new property listing
        /// </summary>
        /// <param name="property">The property to create</param>
        /// <returns>The created property</returns>
        /// <response code="201">Returns the newly created property</response>
        /// <response code="400">If the property data is invalid</response>
        /// <response code="401">If the user is not authenticated</response>
        [HttpPost]
        public async Task<ActionResult<Property>> Post([FromBody] Property property)
        {
            await _propertyService.CreateAsync(property);
            return CreatedAtAction(nameof(Get), new { id = property.Id }, property);
        }

        /// <summary>
        /// Updates an existing property
        /// </summary>
        /// <param name="id">The ID of the property to update</param>
        /// <param name="property">The updated property data</param>
        /// <returns>No content</returns>
        /// <response code="204">If the property was successfully updated</response>
        /// <response code="404">If the property is not found</response>
        /// <response code="401">If the user is not authenticated</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] Property property)
        {
            if (id != property.Id)
                return BadRequest();

            try
            {
                await _propertyService.UpdateAsync(id, property);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return NoContent();
        }

        /// <summary>
        /// Deletes a specific property
        /// </summary>
        /// <param name="id">The ID of the property to delete</param>
        /// <returns>No content</returns>
        /// <response code="204">If the property was successfully deleted</response>
        /// <response code="404">If the property is not found</response>
        /// <response code="401">If the user is not authenticated</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _propertyService.RemoveAsync(id);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return NoContent();
        }

        /// <summary>
        /// Searches for properties by location
        /// </summary>
        /// <param name="location">The location to search for properties</param>
        /// <returns>List of properties matching the location</returns>
        /// <response code="200">Returns the list of properties</response>
        [HttpGet("search")]
        public async Task<ActionResult<List<Property>>> Search([FromQuery] string location)
        {
            try
            {
                var properties = await _propertyService.SearchAsync(location);
                return Ok(properties);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching properties by location: {Location}", location);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Gets properties by type
        /// </summary>
        /// <param name="type">The type of properties to retrieve</param>
        /// <returns>List of properties matching the type</returns>
        /// <response code="200">Returns the list of properties</response>
        [HttpGet("type/{type}")]
        public async Task<ActionResult<List<Property>>> GetByType(string type)
        {
            try
            {
                var properties = await _propertyService.GetByTypeAsync(type);
                return Ok(properties);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting properties by type: {Type}", type);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Gets properties by price range
        /// </summary>
        /// <param name="minPrice">Minimum price</param>
        /// <param name="maxPrice">Maximum price</param>
        /// <returns>List of properties within the price range</returns>
        /// <response code="200">Returns the list of properties</response>
        [HttpGet("price-range")]
        public async Task<ActionResult<List<Property>>> GetByPriceRange(
            [FromQuery] decimal minPrice,
            [FromQuery] decimal maxPrice)
        {
            try
            {
                var properties = await _propertyService.GetByPriceRangeAsync(minPrice, maxPrice);
                return Ok(properties);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting properties by price range: {MinPrice} - {MaxPrice}", 
                    minPrice, maxPrice);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Gets properties by user ID
        /// </summary>
        /// <param name="userId">The ID of the user</param>
        /// <returns>List of properties owned by the user</returns>
        /// <response code="200">Returns the list of properties</response>
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<Property>>> GetByUserId(string userId)
        {
            try
            {
                var properties = await _propertyService.GetPropertiesByUserIdAsync(userId);
                return Ok(properties);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting properties by user ID: {UserId}", userId);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
