# API Documentation

Welcome to the API documentation for RealEstate.Tests. This section contains detailed documentation for all test classes and methods.

## Test Namespaces

### RealEstate.Tests.Controllers
Contains test classes for API controllers:
- AuthControllerTests
- PropertyControllerTests
- PropertiesControllerTests
- FavoriteControllerTests

### Test Categories

#### Authentication Tests
Tests for user authentication including:
- Registration
- Login
- Token validation

#### Property Management Tests
Tests for property operations including:
- CRUD operations
- Search functionality
- Filtering

#### Favorite Management Tests
Tests for favorite property management including:
- Adding favorites
- Removing favorites
- Listing favorites

## Common Test Patterns

### Setup
Most test classes follow this setup pattern:
```csharp
public class TestClass
{
    private readonly Mock<IService> _mockService;
    private readonly Controller _controller;

    public TestClass()
    {
        _mockService = new Mock<IService>();
        _controller = new Controller(_mockService.Object);
    }
}
```

### Test Method Structure
Test methods follow this pattern:
```csharp
[Fact]
public async Task MethodName_Scenario_ExpectedResult()
{
    // Arrange
    var testData = new TestData();

    // Act
    var result = await _controller.Method(testData);

    // Assert
    Assert.IsType<ExpectedType>(result);
}
```
