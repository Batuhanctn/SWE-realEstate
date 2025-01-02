using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Moq;
using RealEstate.API.Controllers;
using RealEstate.API.Data;
using RealEstate.API.Models;
using System.Security.Claims;
using static RealEstate.API.Controllers.FavoriteController;

namespace RealEstate.Tests.Controllers
{
    /// <summary>
    /// Test suite for favorite property management functionality
    /// </summary>
    /// <remarks>
    /// Contains comprehensive unit tests for favorite-related operations including
    /// adding, removing, and retrieving favorite properties for users
    /// </remarks>
    public class FavoriteControllerTests
    {
        private readonly Mock<ILogger<FavoriteController>> _mockLogger;
        private readonly Mock<IMongoCollection<Favorite>> _mockFavorites;
        private readonly Mock<IMongoCollection<Property>> _mockProperties;
        private readonly FavoriteController _controller;
        private const string TEST_USER_ID = "test-user-id";

        /// <summary>
        /// Initializes a new instance of the FavoriteControllerTests class
        /// </summary>
        /// <remarks>
        /// Sets up the test environment with mock logger, collections, and user authentication
        /// to facilitate favorite property management testing
        /// </remarks>
        public FavoriteControllerTests()
        {
            _mockLogger = new Mock<ILogger<FavoriteController>>();
            _mockFavorites = new Mock<IMongoCollection<Favorite>>();
            _mockProperties = new Mock<IMongoCollection<Property>>();

            // Create a real context with mocked collections
            var context = new TestRealEstateDbContext(_mockFavorites.Object, _mockProperties.Object);
            _controller = new FavoriteController(context, _mockLogger.Object);

            // Setup ClaimsPrincipal for authorization
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, TEST_USER_ID)
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };
        }

        /// <summary>
        /// Test implementation of RealEstateDbContext for testing
        /// </summary>
        /// <remarks>
        /// Provides a test-specific implementation of the RealEstateDbContext class
        /// to facilitate unit testing of favorite property management functionality
        /// </remarks>
        private class TestRealEstateDbContext : RealEstateDbContext
        {
            private readonly IMongoCollection<Favorite> _favorites;
            private readonly IMongoCollection<Property> _properties;

            public TestRealEstateDbContext(
                IMongoCollection<Favorite> favorites,
                IMongoCollection<Property> properties) : base(null!)
            {
                _favorites = favorites;
                _properties = properties;
            }

            /// <summary>
            /// Gets the collection of favorite properties
            /// </summary>
            public override IMongoCollection<Property> Properties => _properties;

            /// <summary>
            /// Gets the collection of favorite properties
            /// </summary>
            public override IMongoCollection<Favorite> Favorites => _favorites;
        }

        /// <summary>
        /// Tests that GetFavorites returns user's favorite properties
        /// </summary>
        /// <remarks>
        /// Verifies that the GetFavorites method returns a list of favorite properties
        /// associated with the currently authenticated user
        /// </remarks>
        [Fact]
        public async Task GetFavorites_ReturnsOkResult_WithProperties()
        {
            // Arrange
            var favorites = new List<Favorite>
            {
                new Favorite { UserId = TEST_USER_ID, PropertyId = "prop1" },
                new Favorite { UserId = TEST_USER_ID, PropertyId = "prop2" }
            };

            var properties = new List<Property>
            {
                new Property { Id = "prop1", Title = "Property 1" },
                new Property { Id = "prop2", Title = "Property 2" }
            };

            var mockFavoriteCursor = new Mock<IAsyncCursor<Favorite>>();
            mockFavoriteCursor.Setup(x => x.Current).Returns(favorites);
            mockFavoriteCursor
                .SetupSequence(x => x.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);

            var mockPropertyCursor = new Mock<IAsyncCursor<Property>>();
            mockPropertyCursor.Setup(x => x.Current).Returns(properties);
            mockPropertyCursor
                .SetupSequence(x => x.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);

            _mockFavorites.Setup(x => x.FindAsync(
                It.IsAny<FilterDefinition<Favorite>>(),
                It.IsAny<FindOptions<Favorite>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockFavoriteCursor.Object);

            _mockProperties.Setup(x => x.FindAsync(
                It.IsAny<FilterDefinition<Property>>(),
                It.IsAny<FindOptions<Property>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockPropertyCursor.Object);

            // Act
            var result = await _controller.GetFavorites();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedProperties = Assert.IsAssignableFrom<IEnumerable<Property>>(okResult.Value);
            Assert.Equal(2, returnedProperties.Count());
        }

        /// <summary>
        /// Tests that AddToFavorites adds property to favorites
        /// </summary>
        /// <remarks>
        /// Verifies that the AddToFavorites method successfully adds a property to the user's favorites
        /// </remarks>
        [Fact]
        public async Task AddToFavorites_WithValidRequest_ReturnsOkResult()
        {
            // Arrange
            var request = new AddFavoriteRequest { propertyId = "test-property-id" };
            var property = new Property { Id = "test-property-id", Title = "Test Property" };

            var mockPropertyCursor = new Mock<IAsyncCursor<Property>>();
            mockPropertyCursor.Setup(x => x.Current).Returns(new List<Property> { property });
            mockPropertyCursor
                .SetupSequence(x => x.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);

            var mockFavoriteCursor = new Mock<IAsyncCursor<Favorite>>();
            mockFavoriteCursor.Setup(x => x.Current).Returns(new List<Favorite>());
            mockFavoriteCursor
                .SetupSequence(x => x.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _mockProperties.Setup(x => x.FindAsync(
                It.IsAny<FilterDefinition<Property>>(),
                It.IsAny<FindOptions<Property>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockPropertyCursor.Object);

            _mockFavorites.Setup(x => x.FindAsync(
                It.IsAny<FilterDefinition<Favorite>>(),
                It.IsAny<FindOptions<Favorite>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockFavoriteCursor.Object);

            _mockFavorites.Setup(x => x.InsertOneAsync(
                It.IsAny<Favorite>(),
                It.IsAny<InsertOneOptions>(),
                It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.AddToFavorites(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
        }

        /// <summary>
        /// Tests that AddToFavorites returns BadRequest for null request
        /// </summary>
        /// <remarks>
        /// Verifies that the AddToFavorites method returns a BadRequest response when a null request is provided
        /// </remarks>
        [Fact]
        public async Task AddToFavorites_WithNullRequest_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.AddToFavorites(null!);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        /// <summary>
        /// Tests that RemoveFromFavorites removes property from favorites
        /// </summary>
        /// <remarks>
        /// Verifies that the RemoveFromFavorites method successfully removes a property from the user's favorites
        /// </remarks>
        [Fact]
        public async Task RemoveFromFavorites_ExistingFavorite_ReturnsNoContent()
        {
            // Arrange
            var propertyId = "test-property-id";
            Console.WriteLine("buraya geldi");
            _mockFavorites.Setup(x => x.DeleteOneAsync(
                It.IsAny<FilterDefinition<Favorite>>(),
                null,
                default))
                .ReturnsAsync(new DeleteResult.Acknowledged(1));

            // Act
            Console.WriteLine("buraya geldi");
            var result = await _controller.RemoveFromFavorites(propertyId);
            Console.WriteLine(result);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        /// <summary>
        /// Tests that CheckIsFavorite checks if property is in favorites
        /// </summary>
        /// <remarks>
        /// Verifies that the CheckIsFavorite method correctly determines whether a property is in the user's favorites
        /// </remarks>
        [Fact]
        public async Task CheckIsFavorite_ExistingFavorite_ReturnsTrue()
        {
            // Arrange
            var propertyId = "test-property-id";
            var favorite = new Favorite { Id = "favorite-id", UserId = TEST_USER_ID, PropertyId = propertyId };

            var mockFavoriteCursor = new Mock<IAsyncCursor<Favorite>>();
            mockFavoriteCursor.Setup(x => x.Current).Returns(new List<Favorite> { favorite });
            mockFavoriteCursor
                .SetupSequence(x => x.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);

            _mockFavorites.Setup(x => x.FindAsync(
                It.IsAny<FilterDefinition<Favorite>>(),
                It.IsAny<FindOptions<Favorite>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockFavoriteCursor.Object);

            // Act
            var result = await _controller.CheckIsFavorite(propertyId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.True((bool)okResult.Value!);
        }

        /// <summary>
        /// Tests that CheckIsFavorite returns False for non-existing favorite
        /// </summary>
        /// <remarks>
        /// Verifies that the CheckIsFavorite method correctly returns False when a property is not in the user's favorites
        /// </remarks>
        [Fact]
        public async Task CheckIsFavorite_NonExistingFavorite_ReturnsFalse()
        {
            // Arrange
            var propertyId = "non-existing-property";

            var mockFavoriteCursor = new Mock<IAsyncCursor<Favorite>>();
            mockFavoriteCursor.Setup(x => x.Current).Returns(new List<Favorite>());
            mockFavoriteCursor
                .SetupSequence(x => x.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _mockFavorites.Setup(x => x.FindAsync(
                It.IsAny<FilterDefinition<Favorite>>(),
                It.IsAny<FindOptions<Favorite>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockFavoriteCursor.Object);

            // Act
            var result = await _controller.CheckIsFavorite(propertyId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.False((bool)okResult.Value!);
        }
    }
}