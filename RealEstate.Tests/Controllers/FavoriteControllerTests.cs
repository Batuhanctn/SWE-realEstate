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
    /// Test suite for FavoriteController
    /// Contains unit tests for favorite property operations
    /// </summary>
    public class FavoriteControllerTests
    {
        private readonly Mock<ILogger<FavoriteController>> _mockLogger;
        private readonly Mock<IMongoCollection<Favorite>> _mockFavorites;
        private readonly Mock<IMongoCollection<Property>> _mockProperties;
        private readonly FavoriteController _controller;
        private const string TEST_USER_ID = "test-user-id";

        /// <summary>
        /// Initializes a new instance of FavoriteControllerTests
        /// Sets up mocks and controller instance for testing
        /// </summary>
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

        // Test implementation of RealEstateDbContext for testing
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

            public override IMongoCollection<Property> Properties => _properties;
            public override IMongoCollection<Favorite> Favorites => _favorites;
        }

        /// <summary>
        /// Tests that GetFavorites returns user's favorite properties
        /// </summary>
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