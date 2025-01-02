# RealEstate.Tests Documentation

Welcome to the RealEstate.Tests documentation. This documentation covers the test suite for the RealEstate application.

## Test Categories

The test suite is organized into the following categories:

### Authentication Tests
Tests for user authentication functionality including:
- User registration
- User login
- Token validation

### Property Management Tests
Tests for property-related operations including:
- Creating properties
- Updating properties
- Deleting properties
- Searching properties
- Filtering properties

### Favorites Tests
Tests for favorite property management including:
- Adding properties to favorites
- Removing properties from favorites
- Retrieving favorite properties

## Getting Started

To run the tests:

1. Clone the repository
2. Navigate to the RealEstate.Tests directory
3. Run `dotnet test`

## Test Structure

Each test class follows a similar pattern:
- Setup code in the constructor
- Individual test methods for specific scenarios
- Mock objects for external dependencies
- Assertions to verify expected behavior

## Contributing

When adding new tests:
1. Follow the existing naming convention
2. Add XML documentation comments
3. Mock external dependencies
4. Test both success and failure scenarios
