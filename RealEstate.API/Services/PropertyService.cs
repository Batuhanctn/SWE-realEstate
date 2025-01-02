using RealEstate.API.Models;
using MongoDB.Driver;
using System.Linq.Expressions;
using Microsoft.Extensions.Logging;

namespace RealEstate.API.Services
{
    /// <summary>
    /// Service class for handling property-related operations
    /// Implements IPropertyService interface
    /// </summary>
    public class PropertyService : IPropertyService
    {
        private readonly IMongoCollection<Property> _properties;
        private readonly ILogger<PropertyService> _logger;

        /// <summary>
        /// Initializes a new instance of the PropertyService class
        /// </summary>
        /// <param name="database">The database context for accessing property data</param>
        /// <param name="logger">The logger instance for logging operations</param>
        public PropertyService(IMongoDatabase database, ILogger<PropertyService> logger)
        {
            _properties = database.GetCollection<Property>("Properties");
            _logger = logger;

            // Ensure indexes
            var indexKeysDefinition = Builders<Property>.IndexKeys.Ascending(p => p.Location);
            var indexOptions = new CreateIndexOptions { Name = "Location_Index" };
            var indexModel = new CreateIndexModel<Property>(indexKeysDefinition, indexOptions);
            _properties.Indexes.CreateOne(indexModel);
        }

        /// <summary>
        /// Gets all properties from the database
        /// </summary>
        public async Task<List<Property>> GetAsync()
        {
            try
            {
                // If collection is empty, seed sample data
                if (!await _properties.Find(_ => true).AnyAsync())
                {
                    await SeedSampleData();
                }

                return await _properties.Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting properties");
                throw;
            }
        }

        /// <summary>
        /// Gets a property by ID
        /// </summary>
        public async Task<Property?> GetAsync(string id)
        {
            try
            {
                return await _properties.Find(x => x.Id == id).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting property with ID: {Id}", id);
                throw;
            }
        }

        /// <summary>
        /// Creates a new property
        /// </summary>
        public async Task CreateAsync(Property property)
        {
            try
            {
                await _properties.InsertOneAsync(property);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating property");
                throw;
            }
        }

        /// <summary>
        /// Updates an existing property
        /// </summary>
        public async Task UpdateAsync(string id, Property property)
        {
            try
            {
                await _properties.ReplaceOneAsync(x => x.Id == id, property);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating property with ID: {Id}", id);
                throw;
            }
        }

        /// <summary>
        /// Removes a property
        /// </summary>
        public async Task RemoveAsync(string id)
        {
            try
            {
                await _properties.DeleteOneAsync(x => x.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while removing property with ID: {Id}", id);
                throw;
            }
        }

        /// <summary>
        /// Gets properties by type
        /// </summary>
        public async Task<List<Property>> GetByTypeAsync(string type)
        {
            try
            {
                var filter = Builders<Property>.Filter.Eq(p => p.Type, type);
                return await _properties.Find(filter).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting properties by type: {Type}", type);
                throw;
            }
        }

        /// <summary>
        /// Gets properties by price range
        /// </summary>
        public async Task<List<Property>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            try
            {
                var filter = Builders<Property>.Filter.And(
                    Builders<Property>.Filter.Gte(p => p.Price, minPrice),
                    Builders<Property>.Filter.Lte(p => p.Price, maxPrice)
                );
                return await _properties.Find(filter).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting properties by price range: {MinPrice} - {MaxPrice}", minPrice, maxPrice);
                throw;
            }
        }

        /// <summary>
        /// Searches properties by location
        /// </summary>
        public async Task<List<Property>> SearchAsync(string location)
        {
            try
            {
                var filter = Builders<Property>.Filter.Regex(p => p.Location, new MongoDB.Bson.BsonRegularExpression(location, "i"));
                return await _properties.Find(filter).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching properties by location: {Location}", location);
                throw;
            }
        }

        /// <summary>
        /// Gets properties by user ID
        /// </summary>
        public async Task<List<Property>> GetPropertiesByUserIdAsync(string userId)
        {
            try
            {
                return await _properties.Find(p => p.UserId == userId).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting properties by user ID: {UserId}", userId);
                throw;
            }
        }

        /// <summary>
        /// Gets filtered properties based on various criteria
        /// </summary>
        public async Task<List<Property>> GetFilteredPropertiesAsync(
            string? city = null, string? propertyType = null,
            decimal? minPrice = null, decimal? maxPrice = null,
            int? bedrooms = null, int? bathrooms = null,
            double? minSquareMeters = null, double? maxSquareMeters = null)
        {
            try
            {
                var builder = Builders<Property>.Filter;
                var filters = new List<FilterDefinition<Property>>();

                if (!string.IsNullOrEmpty(city))
                    filters.Add(builder.Regex(p => p.City, new MongoDB.Bson.BsonRegularExpression(city, "i")));

                if (!string.IsNullOrEmpty(propertyType))
                    filters.Add(builder.Eq(p => p.Type, propertyType));

                if (minPrice.HasValue)
                    filters.Add(builder.Gte(p => p.Price, minPrice.Value));

                if (maxPrice.HasValue)
                    filters.Add(builder.Lte(p => p.Price, maxPrice.Value));

                if (bedrooms.HasValue)
                    filters.Add(builder.Eq(p => p.Bedrooms, bedrooms.Value));

                if (bathrooms.HasValue)
                    filters.Add(builder.Eq(p => p.Bathrooms, bathrooms.Value));

                if (minSquareMeters.HasValue)
                    filters.Add(builder.Gte(p => p.SquareMeters, minSquareMeters.Value));

                if (maxSquareMeters.HasValue)
                    filters.Add(builder.Lte(p => p.SquareMeters, maxSquareMeters.Value));

                var filter = filters.Count > 0 
                    ? builder.And(filters) 
                    : builder.Empty;

                return await _properties.Find(filter).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting filtered properties");
                throw;
            }
        }

        private async Task SeedSampleData()
        {
            var sampleProperties = new List<Property>
            {
                new Property
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = "Modern Apartment in City Center",
                    Description = "Beautiful modern apartment with great views",
                    Type = "Apartment",
                    Price = 250000M,
                    Location = "City Center",
                    City = "New York",
                    Bedrooms = 2,
                    Bathrooms = 1,
                    SquareMeters = 75,
                    UserId = "sample-user-1"
                },
                new Property
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = "Suburban Family Home",
                    Description = "Spacious family home with garden",
                    Type = "House",
                    Price = 450000M,
                    Location = "Suburbs",
                    City = "Los Angeles",
                    Bedrooms = 4,
                    Bathrooms = 2,
                    SquareMeters = 200,
                    UserId = "sample-user-2"
                }
            };

            await _properties.InsertManyAsync(sampleProperties);
        }
    }
}
