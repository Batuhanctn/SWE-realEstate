# Favorite Property Tests

## Overview
The favorite property tests verify the functionality of managing user's favorite properties.

## Test Classes

### FavoriteControllerTests
Located in `Controllers/FavoriteControllerTests.cs`, this class contains tests for:

#### Favorite Management
- `GetFavorites_ReturnsOkResult_WithProperties`: Tests retrieving user's favorites
- `AddToFavorites_WithValidRequest_ReturnsOkResult`: Verifies adding to favorites
- `AddToFavorites_WithNullRequest_ReturnsBadRequest`: Tests invalid input handling
- `RemoveFromFavorites_ExistingFavorite_ReturnsNoContent`: Tests removing from favorites
- `CheckIsFavorite_ExistingFavorite_ReturnsTrue`: Verifies favorite status checking

## Test Setup
The tests use:
- Mock favorite collection
- Mock property collection
- Mock logger
- Test user authentication context

## Common Patterns
1. Arrange: Set up test data and mock collections
2. Act: Call the controller method
3. Assert: Verify the response and database operations

## Example
```csharp
[Fact]
public async Task AddToFavorites_WithValidRequest_ReturnsOkResult()
{
    // Arrange
    var request = new AddToFavoritesRequest
    {
        PropertyId = "test-property-id"
    };

    // Act
    var result = await _controller.AddToFavorites(request);

    // Assert
    Assert.IsType<OkResult>(result);
}
```

## Test Data
The tests use common test data including:
- Test user ID: "test-user-id"
- Test property IDs
- Mock favorite entries

## Error Cases
Tests cover various error scenarios:
- Invalid requests
- Non-existent properties
- Duplicate favorites
- Unauthorized access
