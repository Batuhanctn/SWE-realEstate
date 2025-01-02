using RealEstate.API.Models;

namespace RealEstate.API.Services
{
    /// <summary>
    /// Service interface for managing properties
    /// </summary>
    public interface IPropertyService
    {
        /// <summary>
        /// Gets all properties
        /// </summary>
        Task<List<Property>> GetAsync();

        /// <summary>
        /// Gets a property by ID
        /// </summary>
        Task<Property?> GetAsync(string id);

        /// <summary>
        /// Creates a new property
        /// </summary>
        Task CreateAsync(Property property);

        /// <summary>
        /// Updates an existing property
        /// </summary>
        Task UpdateAsync(string id, Property property);

        /// <summary>
        /// Removes a property
        /// </summary>
        Task RemoveAsync(string id);

        /// <summary>
        /// Gets properties by type
        /// </summary>
        Task<List<Property>> GetByTypeAsync(string type);

        /// <summary>
        /// Gets properties by price range
        /// </summary>
        Task<List<Property>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice);

        /// <summary>
        /// Searches properties by location
        /// </summary>
        Task<List<Property>> SearchAsync(string location);

        /// <summary>
        /// Gets properties by user ID
        /// </summary>
        Task<List<Property>> GetPropertiesByUserIdAsync(string userId);

        /// <summary>
        /// Gets filtered properties based on various criteria
        /// </summary>
        Task<List<Property>> GetFilteredPropertiesAsync(string? city = null, string? propertyType = null, 
            decimal? minPrice = null, decimal? maxPrice = null, int? bedrooms = null, int? bathrooms = null, 
            double? minSquareMeters = null, double? maxSquareMeters = null);
    }
}
