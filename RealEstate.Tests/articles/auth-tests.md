# Authentication Tests

## Overview
The authentication tests verify the functionality of user registration, login, and token validation in the RealEstate application.

## Test Classes

### AuthControllerTests
Located in `Controllers/AuthControllerTests.cs`, this class contains tests for:

#### User Registration
- `Register_ValidUser_ReturnsOk`: Verifies successful user registration
- `Register_ExistingEmail_ReturnsBadRequest`: Tests duplicate email handling
- `Register_InvalidModel_ReturnsBadRequest`: Validates input model requirements

#### User Login
- `Login_ValidCredentials_ReturnsToken`: Tests successful login with token generation
- `Login_InvalidEmail_ReturnsBadRequest`: Verifies invalid email handling
- `Login_WrongPassword_ReturnsBadRequest`: Tests incorrect password scenarios

## Test Setup
The tests use:
- Mock MongoDB database
- Mock configuration for JWT settings
- Mock logger for testing error scenarios

## Common Patterns
1. Arrange: Set up test data and mocks
2. Act: Call the controller method
3. Assert: Verify the response and any side effects

## Example
```csharp
[Fact]
public async Task Register_ValidUser_ReturnsOk()
{
    // Arrange
    var user = new RegisterRequest 
    { 
        Email = "test@example.com",
        Password = "Test123!",
        FirstName = "Test",
        LastName = "User"
    };

    // Act
    var result = await _controller.Register(user);

    // Assert
    Assert.IsType<OkObjectResult>(result);
}
```
