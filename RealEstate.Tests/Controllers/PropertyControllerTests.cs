using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using RealEstate.API.Controllers;
using RealEstate.API.Models;
using RealEstate.API.Services;
using Xunit;

namespace RealEstate.Tests.Controllers;

/// <summary>
/// Test suite for PropertyController
/// Contains unit tests for property-related operations
/// </summary>
public class PropertyControllerTests
{
    private readonly Mock<IPropertyService> _mockPropertyService;
    private readonly Mock<ILogger<PropertyController>> _mockLogger;
    private readonly PropertyController _controller;

    /// <summary>
    /// Initializes a new instance of PropertyControllerTests
    /// Sets up mocks and controller instance for testing
    /// </summary>
    public PropertyControllerTests()
    {
        _mockPropertyService = new Mock<IPropertyService>();
        _mockLogger = new Mock<ILogger<PropertyController>>();
        _controller = new PropertyController(_mockPropertyService.Object, _mockLogger.Object);
    }

    /// <summary>
    /// Tests that GetAsync returns all properties successfully
    /// </summary>
    [Fact]
    public async Task Get_ReturnsOkResult_WithListOfProperties()
    {
        // Arrange
        var expectedProperties = new List<Property>
        {
            new() { Id = "1", Title = "Test Property 1", Price = 100000 },
            new() { Id = "2", Title = "Test Property 2", Price = 200000 }
        };

        _mockPropertyService.Setup(x => x.GetAsync())
            .ReturnsAsync(expectedProperties);

        // Act
        var result = await _controller.Get();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<List<Property>>(okResult.Value);
        Assert.Equal(2, returnValue.Count);
        Assert.Equal(expectedProperties[0].Id, returnValue[0].Id);
        Assert.Equal(expectedProperties[1].Id, returnValue[1].Id);
    }

    /// <summary>
    /// Tests that GetAsync with ID returns specific property
    /// </summary>
    [Fact]
    public async Task GetById_ReturnsOkResult_WhenPropertyExists()
    {
        // Arrange
        var propertyId = "507f1f77bcf86cd799439011";
        var expectedProperty = new Property 
        { 
            Id = propertyId, 
            Title = "Test Property", 
            Price = 100000 
        };

        _mockPropertyService.Setup(x => x.GetAsync(propertyId))
            .ReturnsAsync(expectedProperty);

        // Act
        var result = await _controller.Get(propertyId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<Property>(okResult.Value);
        Assert.Equal(propertyId, returnValue.Id);
    }

    /// <summary>
    /// Tests that GetAsync with ID returns NotFound when property does not exist
    /// </summary>
    [Fact]
    public async Task GetById_ReturnsNotFound_WhenPropertyDoesNotExist()
    {
        // Arrange
        var propertyId = "507f1f77bcf86cd799439011";
        _mockPropertyService.Setup(x => x.GetAsync(propertyId))
            .ReturnsAsync((Property)null);

        // Act
        var result = await _controller.Get(propertyId);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    /// <summary>
    /// Tests that PostAsync creates property successfully
    /// </summary>
    [Fact]
    public async Task Post_ReturnsCreatedAtAction_WhenPropertyIsValid()
    {
        // Arrange
        var newProperty = new Property 
        { 
            Title = "New Property",
            Price = 150000,
            Type = "House",
            Location = "Test Location"
        };

        _mockPropertyService.Setup(x => x.CreateAsync(It.IsAny<Property>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Post(newProperty);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(PropertyController.Get), createdAtActionResult.ActionName);
        var returnValue = Assert.IsType<Property>(createdAtActionResult.Value);
        Assert.Equal(newProperty.Title, returnValue.Title);
    }

    /// <summary>
    /// Tests that UpdateAsync updates property successfully
    /// </summary>
    [Fact]
    public async Task Update_ReturnsNoContent_WhenPropertyExists()
    {
        // Arrange
        var propertyId = "507f1f77bcf86cd799439011";
        var existingProperty = new Property 
        { 
            Id = propertyId,
            Title = "Existing Property"
        };
        var updatedProperty = new Property 
        { 
            Title = "Updated Property",
            Price = 200000
        };

        _mockPropertyService.Setup(x => x.GetAsync(propertyId))
            .ReturnsAsync(existingProperty);
        _mockPropertyService.Setup(x => x.UpdateAsync(propertyId, It.IsAny<Property>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Put(propertyId, updatedProperty);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    /// <summary>
    /// Tests that UpdateAsync returns NotFound when property does not exist
    /// </summary>
    [Fact]
    public async Task Update_ReturnsNotFound_WhenPropertyDoesNotExist()
    {
        // Arrange
        var propertyId = "507f1f77bcf86cd799439011";
        var updatedProperty = new Property 
        { 
            Title = "Updated Property" 
        };

        _mockPropertyService.Setup(x => x.GetAsync(propertyId))
            .ReturnsAsync((Property)null);

        // Act
        var result = await _controller.Put(propertyId, updatedProperty);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    /// <summary>
    /// Tests that DeleteAsync removes property successfully
    /// </summary>
    [Fact]
    public async Task Delete_ReturnsNoContent_WhenPropertyExists()
    {
        // Arrange
        var propertyId = "507f1f77bcf86cd799439011";
        var existingProperty = new Property 
        { 
            Id = propertyId,
            Title = "Property to Delete"
        };

        _mockPropertyService.Setup(x => x.GetAsync(propertyId))
            .ReturnsAsync(existingProperty);
        _mockPropertyService.Setup(x => x.RemoveAsync(propertyId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Delete(propertyId);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    /// <summary>
    /// Tests that DeleteAsync returns NotFound when property does not exist
    /// </summary>
    [Fact]
    public async Task Delete_ReturnsNotFound_WhenPropertyDoesNotExist()
    {
        // Arrange
        var propertyId = "507f1f77bcf86cd799439011";
        _mockPropertyService.Setup(x => x.GetAsync(propertyId))
            .ReturnsAsync((Property)null);

        // Act
        var result = await _controller.Delete(propertyId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    /// <summary>
    /// Tests that SearchAsync returns properties by location
    /// </summary>
    [Fact]
    public async Task Search_ReturnsOkResult_WithMatchingProperties()
    {
        // Arrange
        var location = "Test Location";
        var expectedProperties = new List<Property>
        {
            new() { Id = "1", Title = "Property 1", Location = location },
            new() { Id = "2", Title = "Property 2", Location = location }
        };

        _mockPropertyService.Setup(x => x.SearchAsync(location))
            .ReturnsAsync(expectedProperties);

        // Act
        var result = await _controller.Search(location);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<List<Property>>(okResult.Value);
        Assert.Equal(2, returnValue.Count);
        Assert.All(returnValue, p => Assert.Equal(location, p.Location));
    }

    /// <summary>
    /// Tests that GetByPriceRangeAsync returns properties within price range
    /// </summary>
    [Fact]
    public async Task GetByPriceRange_ReturnsOkResult_WithPropertiesInRange()
    {
        // Arrange
        decimal minPrice = 100000;
        decimal maxPrice = 200000;
        var expectedProperties = new List<Property>
        {
            new() { Id = "1", Title = "Property 1", Price = 150000 },
            new() { Id = "2", Title = "Property 2", Price = 175000 }
        };

        _mockPropertyService.Setup(x => x.GetByPriceRangeAsync(minPrice, maxPrice))
            .ReturnsAsync(expectedProperties);

        // Act
        var result = await _controller.GetByPriceRange(minPrice, maxPrice);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<List<Property>>(okResult.Value);
        Assert.Equal(2, returnValue.Count);
        Assert.All(returnValue, p => Assert.InRange(p.Price, minPrice, maxPrice));
    }
}
