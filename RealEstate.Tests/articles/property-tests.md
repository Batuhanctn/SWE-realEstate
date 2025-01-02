# Property Management Tests

## Overview
The property management tests verify CRUD operations and search functionality for real estate properties.

## Test Classes

### PropertyControllerTests
Located in `Controllers/PropertyControllerTests.cs`, this class tests:

#### Property Retrieval
- `Get_ReturnsOkResult_WithListOfProperties`: Tests fetching all properties
- `GetById_ReturnsOkResult_WhenPropertyExists`: Verifies single property retrieval
- `GetById_ReturnsNotFound_WhenPropertyDoesNotExist`: Tests missing property handling

#### Property Creation
- `Post_ReturnsCreatedAtAction_WhenPropertyIsValid`: Tests property creation
- `Post_ReturnsBadRequest_WhenModelStateIsInvalid`: Validates input requirements

#### Property Updates
- `Put_ReturnsNoContent_WhenPropertyExists`: Tests successful updates
- `Put_ReturnsNotFound_WhenPropertyDoesNotExist`: Verifies missing property handling

#### Property Deletion
- `Delete_ReturnsNoContent_WhenPropertyExists`: Tests successful deletion
- `Delete_ReturnsNotFound_WhenPropertyDoesNotExist`: Verifies missing property handling

#### Search and Filtering
- `Search_ReturnsOkResult_WithMatchingProperties`: Tests property search
- `GetByPriceRange_ReturnsOkResult_WithPropertiesInRange`: Tests price filtering

## Test Setup
The tests use:
- Mock property service
- Mock logger
- Test data fixtures

## Common Patterns
1. Arrange: Prepare test data and configure mocks
2. Act: Execute controller action
3. Assert: Verify response and state changes

## Example
```csharp
[Fact]
public async Task Post_ReturnsCreatedAtAction_WhenPropertyIsValid()
{
    // Arrange
    var property = new Property
    {
        Title = "Test Property",
        Price = 100000,
        Location = "Test City"
    };

    // Act
    var result = await _controller.Post(property);

    // Assert
    var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
    Assert.Equal("Get", createdAtActionResult.ActionName);
}
```
