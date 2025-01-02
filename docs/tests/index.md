# Test Suite Documentation

The test suite provides comprehensive testing coverage for both backend and frontend components.

## Backend Tests

### Controller Tests
- AuthControllerTests
- PropertyControllerTests
- FavoriteControllerTests

### Test Categories

#### Authentication Tests
- User registration
- User login
- Token validation

#### Property Tests
- CRUD operations
- Search functionality
- Filtering

#### Favorite Tests
- Adding to favorites
- Removing from favorites
- Listing favorites

## Frontend Tests

### Component Tests
- Login/Register forms
- Property listing
- Property details
- User profile

### Integration Tests
- API integration
- Authentication flow
- Property management flow

## Test Setup

### Backend Tests
```bash
cd RealEstate.Tests
dotnet test
```

### Frontend Tests
```bash
cd real-estate-client
npm test
```

## Test Coverage

The test suite aims for high code coverage:
- Controllers: 90%+
- Services: 85%+
- Components: 80%+

## Test Patterns

### Arrange-Act-Assert
All tests follow the AAA pattern:
```csharp
// Arrange
var testData = new TestData();

// Act
var result = await controller.Method(testData);

// Assert
Assert.IsType<ExpectedType>(result);
```

### Mocking
External dependencies are mocked using:
- Moq for C# tests
- Jest for JavaScript tests
