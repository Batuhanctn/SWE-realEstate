using MongoDB.Driver;
using RealEstate.API.Models;
using RealEstate.API.Settings;

namespace RealEstate.API.Data
{
    /// <summary>
    /// Database context for the RealEstate application
    /// Manages database connections and entity configurations
    /// </summary>
    public class RealEstateDbContext
    {
        /// <summary>
        /// Initializes a new instance of the RealEstateDbContext
        /// </summary>
        /// <param name="settings">The settings to be used by the context</param>
        public RealEstateDbContext(MongoDbSettings settings)
        {
            if (settings != null)
            {
                var client = new MongoClient(settings.ConnectionString);
                _database = client.GetDatabase(settings.DatabaseName);
            }
        }

        private readonly IMongoDatabase? _database;

        /// <summary>
        /// Gets the Properties collection
        /// Represents the Properties collection in the database
        /// </summary>
        public virtual IMongoCollection<Property> Properties => _database?.GetCollection<Property>("Properties") ?? null!;

        /// <summary>
        /// Gets the Favorites collection
        /// Represents the Favorites collection in the database
        /// </summary>
        public virtual IMongoCollection<Favorite> Favorites => _database?.GetCollection<Favorite>("Favorites") ?? null!;
    }
}
