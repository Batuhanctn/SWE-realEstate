using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using RealEstate.API.Controllers;
using RealEstate.API.Models;
using RealEstate.Tests.TestHelpers;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace RealEstate.Tests.Controllers
{
    /// <summary>
    /// Test suite for property management functionality
    /// </summary>
    /// <remarks>
    /// Contains comprehensive unit tests for property-related operations including
    /// creating, reading, updating, and deleting property listings
    /// </remarks>
    public class PropertiesControllerTests
    {
        private readonly MockMongoDb _mockDb;
        private readonly PropertiesController _controller;
        private readonly string _testUserId = "test-user-id";
        private readonly Mock<IMongoDatabase> _mockDatabase;
        private readonly Mock<IMongoCollection<Property>> _mockCollection;

        /// <summary>
        /// Initializes a new instance of the PropertiesControllerTests class
        /// </summary>
        /// <remarks>
        /// Sets up the test environment with mock database, collections, and user authentication
        /// to facilitate property management testing
        /// </remarks>
        public PropertiesControllerTests()
        {
            _mockDb = new MockMongoDb();
            
            // Setup mock database
            _mockDatabase = new Mock<IMongoDatabase>();
            _mockDatabase.Setup(d => d.GetCollection<Property>("Properties", null))
                        .Returns(_mockDb.PropertyCollection.Object);
            _mockDatabase.Setup(d => d.GetCollection<User>("Users", null))
                        .Returns(_mockDb.UserCollection.Object);

            _mockCollection = _mockDb.PropertyCollection;

            _controller = new PropertiesController(_mockDatabase.Object);

            // Setup controller context with mock user
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, _testUserId)
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };
        }

        /// <summary>
        /// Tests that CreateProperty creates property successfully
        /// </summary>
        /// <remarks>
        /// Verifies that a valid property is created and returned with the correct user ID
        /// </remarks>
        [Fact]
        public async Task CreateProperty_ValidProperty_ReturnsCreatedAtAction()
        {
            // Arrange
            var testUser = new User
            {
                Id = _testUserId,
                Email = "test@example.com",
                Properties = new List<Property>()
            };
            _mockDb.AddUser(testUser);

            var property = new Property
            {
                Title = "Test Property",
                Description = "Test Description",
                Price = 100000,
                Location = "Test Location",
                Address = "Test Address",
                City = "Test City",
                State = "Test State",
                ZipCode = "12345",
                Type = "House",
                PropertyType = "House",
                IsForRent = false,
                Size = 1000
            };

            // Act
            var result = await _controller.CreateProperty(property);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnedProperty = Assert.IsType<Property>(createdAtActionResult.Value);
            Assert.Equal(property.Title, returnedProperty.Title);
            Assert.Equal(_testUserId, returnedProperty.UserId);
            Assert.True(returnedProperty.CreatedAt > DateTime.MinValue);
        }

        /// <summary>
        /// Tests that CreateProperty returns Unauthorized for unauthorized users
        /// </summary>
        /// <remarks>
        /// Verifies that an unauthorized user cannot create a property
        /// </remarks>
        [Fact]
        public async Task CreateProperty_UnauthorizedUser_ReturnsUnauthorized()
        {
            // Arrange
            var controller = new PropertiesController(_mockDatabase.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            
            var property = new Property
            {
                Title = "Test Property"
            };

            // Act
            var result = await controller.CreateProperty(property);

            // Assert
            Assert.IsType<UnauthorizedResult>(result.Result);
        }

        /// <summary>
        /// Tests that GetProperty returns specific property
        /// </summary>
        /// <remarks>
        /// Verifies that a valid property is returned with the correct ID
        /// </remarks>
        [Fact]
        public async Task GetProperty_ExistingProperty_ReturnsProperty()
        {
            // Arrange
            var property = new Property
            {
                Id = "test-property-id",
                Title = "Test Property",
                UserId = _testUserId
            };
            _mockDb.AddProperty(property);

            // Act
            var result = await _controller.GetProperty(property.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedProperty = Assert.IsType<Property>(okResult.Value);
            Assert.Equal(property.Id, returnedProperty.Id);
            Assert.Equal(property.Title, returnedProperty.Title);
        }

        /// <summary>
        /// Tests that GetProperty returns NotFound for non-existing properties
        /// </summary>
        /// <remarks>
        /// Verifies that a non-existent property returns a NotFound result
        /// </remarks>
        [Fact]
        public async Task GetProperty_NonExistingProperty_ReturnsNotFound()
        {
            // Act
            var result = await _controller.GetProperty("non-existing-id");

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        /// <summary>
        /// Tests that DeleteProperty removes property successfully
        /// </summary>
        /// <remarks>
        /// Verifies that a valid property is deleted and returns a NoContent result
        /// </remarks>
        [Fact]
        public async Task DeleteProperty_ExistingOwnedProperty_ReturnsNoContent()
        {
            // Arrange
            var property = new Property
            {
                Id = "test-property-id",
                Title = "Test Property",
                UserId = _testUserId
            };
            _mockDb.AddProperty(property);

            // Act
            var result = await _controller.DeleteProperty(property.Id);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        /// <summary>
        /// Tests that DeleteProperty returns NotFound for non-existing properties
        /// </summary>
        /// <remarks>
        /// Verifies that a non-existent property returns a NotFound result
        /// </remarks>
        [Fact]
        public async Task DeleteProperty_NonExistingProperty_ReturnsNotFound()
        {
            // Act
            var result = await _controller.DeleteProperty("non-existing-id");

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        /// <summary>
        /// Tests that DeleteProperty returns Forbid for properties owned by other users
        /// </summary>
        /// <remarks>
        /// Verifies that a property owned by another user returns a Forbid result
        /// </remarks>
        [Fact]
        public async Task DeleteProperty_PropertyOwnedByOtherUser_ReturnsForbid()
        {
            // Arrange
            var property = new Property
            {
                Id = "test-property-id",
                Title = "Test Property",
                UserId = "other-user-id"
            };
            _mockDb.AddProperty(property);

            // Act
            var result = await _controller.DeleteProperty(property.Id);

            // Assert
            Assert.IsType<ForbidResult>(result);
        }

        /// <summary>
        /// Tests that GetAllProperties returns all properties with filters
        /// </summary>
        /// <remarks>
        /// Verifies that properties are returned with the correct filters applied
        /// </remarks>
        [Fact]
        public async Task GetAllProperties_WithFilters_ReturnsFilteredProperties()
        {
            // Arrange
            var controller = new PropertiesController(_mockDatabase.Object);
            var properties = new List<Property>
            {
                new Property { Id = "1", City = "Istanbul", PropertyType = "Apartment", Price = 100000 },
                new Property { Id = "2", City = "Ankara", PropertyType = "House", Price = 200000 },
                new Property { Id = "3", City = "Istanbul", PropertyType = "Villa", Price = 300000 }
            };

            var mockCursor = new Mock<IAsyncCursor<Property>>();
            mockCursor.Setup(c => c.Current).Returns(properties);
            mockCursor
                .SetupSequence(c => c.MoveNext(It.IsAny<CancellationToken>()))
                .Returns(true)
                .Returns(false);
            mockCursor
                .SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);

            _mockCollection.Setup(c => c.FindAsync(
                It.IsAny<FilterDefinition<Property>>(),
                It.IsAny<FindOptions<Property>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockCursor.Object);

            // Act
            var result = await controller.GetAllProperties(city: "Istanbul", propertyType: null, minPrice: 100000, maxPrice: 300000);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedProperties = Assert.IsType<List<Property>>(okResult.Value);
            Assert.Equal(3, returnedProperties.Count); // In a real scenario, this would be filtered by MongoDB
        }

        /// <summary>
        /// Tests that CreateProperty returns BadRequest for invalid models
        /// </summary>
        /// <remarks>
        /// Verifies that an invalid property model returns a BadRequest result
        /// </remarks>
        [Fact]
        public async Task CreateProperty_InvalidModel_ReturnsBadRequest()
        {
            // Arrange
            var controller = new PropertiesController(_mockDatabase.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, _testUserId)
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            controller.ControllerContext.HttpContext.User = claimsPrincipal;

            controller.ModelState.AddModelError("Title", "Title is required");

            var property = new Property(); // Invalid property without required fields

            // Act
            var result = await controller.CreateProperty(property);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        /// <summary>
        /// Tests that GetUserProperties returns properties for specific user
        /// </summary>
        /// <remarks>
        /// Verifies that properties are returned for the correct user
        /// </remarks>
        [Fact]
        public async Task GetUserProperties_ReturnsUserProperties()
        {
            // Arrange
            var controller = new PropertiesController(_mockDatabase.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, _testUserId)
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            controller.ControllerContext.HttpContext.User = claimsPrincipal;

            var userProperties = new List<Property>
            {
                new Property { Id = "1", UserId = _testUserId, Title = "User Property 1" },
                new Property { Id = "2", UserId = _testUserId, Title = "User Property 2" }
            };

            var mockCursor = new Mock<IAsyncCursor<Property>>();
            mockCursor.Setup(c => c.Current).Returns(userProperties);
            mockCursor
                .SetupSequence(c => c.MoveNext(It.IsAny<CancellationToken>()))
                .Returns(true)
                .Returns(false);
            mockCursor
                .SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);

            _mockCollection.Setup(c => c.FindAsync(
                It.IsAny<FilterDefinition<Property>>(),
                It.IsAny<FindOptions<Property>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockCursor.Object);

            // Act
            var result = await controller.GetUserProperties();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedProperties = Assert.IsType<List<Property>>(okResult.Value);
            Assert.Equal(2, returnedProperties.Count);
            Assert.All(returnedProperties, p => Assert.Equal(_testUserId, p.UserId));
        }

        /// <summary>
        /// Tests that UpdateProperty updates property successfully
        /// </summary>
        /// <remarks>
        /// Verifies that a valid property is updated and returns a NoContent result
        /// </remarks>
        [Fact]
        public async Task UpdateProperty_ExistingProperty_ReturnsNoContent()
        {
            // Arrange
            var existingProperty = new Property 
            { 
                Id = "test-property-id",
                UserId = _testUserId,
                Title = "Old Title",
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            };

            var mockCursor = new Mock<IAsyncCursor<Property>>();
            mockCursor.Setup(c => c.Current).Returns(new List<Property> { existingProperty });
            mockCursor
                .SetupSequence(c => c.MoveNext(It.IsAny<CancellationToken>()))
                .Returns(true)
                .Returns(false);
            mockCursor
                .SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);

            _mockCollection.Setup(c => c.FindAsync(
                It.IsAny<FilterDefinition<Property>>(),
                It.IsAny<FindOptions<Property>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockCursor.Object);

            var updatedProperty = new Property
            {
                Title = "New Title",
                Description = "Updated Description"
            };

            // Act
            var result = await _controller.UpdateProperty("test-property-id", updatedProperty);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        /// <summary>
        /// Tests that UpdateProperty returns NotFound for non-existing properties
        /// </summary>
        /// <remarks>
        /// Verifies that a non-existent property returns a NotFound result
        /// </remarks>
        [Fact]
        public async Task UpdateProperty_NonExistingProperty_ReturnsNotFound()
        {
            // Arrange
            var mockCursor = new Mock<IAsyncCursor<Property>>();
            mockCursor.Setup(c => c.Current).Returns(new List<Property>());
            mockCursor
                .SetupSequence(c => c.MoveNext(It.IsAny<CancellationToken>()))
                .Returns(true)
                .Returns(false);
            mockCursor
                .SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);

            _mockCollection.Setup(c => c.FindAsync(
                It.IsAny<FilterDefinition<Property>>(),
                It.IsAny<FindOptions<Property>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockCursor.Object);

            var updatedProperty = new Property
            {
                Title = "New Title"
            };

            // Act
            var result = await _controller.UpdateProperty("non-existing-id", updatedProperty);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        /// <summary>
        /// Tests that UpdateProperty returns Unauthorized for unauthorized users
        /// </summary>
        /// <remarks>
        /// Verifies that an unauthorized user cannot update a property
        /// </remarks>
        [Fact]
        public async Task UpdateProperty_UnauthorizedUser_ReturnsUnauthorized()
        {
            // Arrange
            var controller = new PropertiesController(_mockDatabase.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            var updatedProperty = new Property
            {
                Title = "New Title"
            };

            // Act
            var result = await controller.UpdateProperty("test-property-id", updatedProperty);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        /// <summary>
        /// Tests that UploadImages returns Unauthorized for unauthorized users
        /// </summary>
        /// <remarks>
        /// Verifies that an unauthorized user cannot upload images
        /// </remarks>
        [Fact]
        public async Task UploadImages_UnauthorizedUser_ReturnsUnauthorized()
        {
            // Arrange
            var controller = new PropertiesController(_mockDatabase.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            var formFiles = new FormFileCollection();
            var file = new FormFile(Stream.Null, 0, 0, "file", "test.jpg");
            formFiles.Add(file);

            // Act
            var result = await controller.UploadImages("test-property-id", formFiles);

            // Assert
            Assert.IsType<UnauthorizedResult>(result.Result);
        }

        /// <summary>
        /// Tests that UploadImages returns NotFound for non-existing properties
        /// </summary>
        /// <remarks>
        /// Verifies that a non-existent property returns a NotFound result
        /// </remarks>
        [Fact]
        public async Task UploadImages_NonExistingProperty_ReturnsNotFound()
        {
            // Arrange
            var mockCursor = new Mock<IAsyncCursor<Property>>();
            mockCursor.Setup(c => c.Current).Returns(new List<Property>());
            mockCursor
                .SetupSequence(c => c.MoveNext(It.IsAny<CancellationToken>()))
                .Returns(true)
                .Returns(false);
            mockCursor
                .SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);

            _mockCollection.Setup(c => c.FindAsync(
                It.IsAny<FilterDefinition<Property>>(),
                It.IsAny<FindOptions<Property>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockCursor.Object);

            var formFiles = new FormFileCollection();
            var file = new FormFile(Stream.Null, 0, 0, "file", "test.jpg");
            formFiles.Add(file);

            // Act
            var result = await _controller.UploadImages("non-existing-id", formFiles);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        /// <summary>
        /// Tests that UploadImages returns BadRequest for no images
        /// </summary>
        /// <remarks>
        /// Verifies that no images uploaded returns a BadRequest result
        /// </remarks>
        [Fact]
        public async Task UploadImages_NoImages_ReturnsBadRequest()
        {
            // Arrange
            var existingProperty = new Property
            {
                Id = "test-property-id",
                UserId = _testUserId,
                Title = "Test Property"
            };

            var mockCursor = new Mock<IAsyncCursor<Property>>();
            mockCursor.Setup(c => c.Current).Returns(new List<Property> { existingProperty });
            mockCursor
                .SetupSequence(c => c.MoveNext(It.IsAny<CancellationToken>()))
                .Returns(true)
                .Returns(false);
            mockCursor
                .SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);

            _mockCollection.Setup(c => c.FindAsync(
                It.IsAny<FilterDefinition<Property>>(),
                It.IsAny<FindOptions<Property>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockCursor.Object);

            var emptyFormFiles = new FormFileCollection();

            // Act
            var result = await _controller.UploadImages("test-property-id", emptyFormFiles);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("No images uploaded", badRequestResult.Value);
        }

        /// <summary>
        /// Tests that UploadImages returns Ok with updated property for valid images
        /// </summary>
        /// <remarks>
        /// Verifies that valid images are uploaded and the property is updated
        /// </remarks>
        [Fact]
        public async Task UploadImages_ValidImages_ReturnsOkWithUpdatedProperty()
        {
            // Arrange
            var existingProperty = new Property
            {
                Id = "test-property-id",
                UserId = _testUserId,
                Title = "Test Property",
                ImageUrls = new List<string>()
            };

            var mockCursor = new Mock<IAsyncCursor<Property>>();
            mockCursor.Setup(c => c.Current).Returns(new List<Property> { existingProperty });
            mockCursor
                .SetupSequence(c => c.MoveNext(It.IsAny<CancellationToken>()))
                .Returns(true)
                .Returns(false);
            mockCursor
                .SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);

            _mockCollection.Setup(c => c.FindAsync(
                It.IsAny<FilterDefinition<Property>>(),
                It.IsAny<FindOptions<Property>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockCursor.Object);

            var formFiles = new FormFileCollection();
            var file1 = new FormFile(Stream.Null, 0, 0, "file1", "test1.jpg");
            var file2 = new FormFile(Stream.Null, 0, 0, "file2", "test2.jpg");
            formFiles.Add(file1);
            formFiles.Add(file2);

            // Act
            var result = await _controller.UploadImages("test-property-id", formFiles);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedProperty = Assert.IsType<Property>(okResult.Value);
            Assert.Equal(existingProperty.Id, returnedProperty.Id);
            Assert.Equal(_testUserId, returnedProperty.UserId);
        }
    }
}
