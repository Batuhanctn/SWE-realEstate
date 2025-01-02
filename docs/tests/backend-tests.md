# Backend Tests

## Controller Tests

### AuthControllerTests

Tests for user authentication endpoints:
- Registration with valid/invalid data
- Login with correct/incorrect credentials
- Token validation

### PropertyControllerTests

Tests for property management:
- Creating properties
- Retrieving properties
- Updating properties
- Deleting properties
- Filtering and searching

### FavoriteControllerTests

Tests for favorite property management:
- Adding to favorites
- Removing from favorites
- Listing favorites

## Service Tests

### AuthServiceTests

Tests for authentication service:
- Password hashing
- Token generation
- User validation

### PropertyServiceTests

Tests for property service:
- CRUD operations
- Search functionality
- Validation rules

### FavoriteServiceTests

Tests for favorite service:
- Favorite management
- User-property relationships
- Validation rules

## Integration Tests

Tests that verify the interaction between different components:
- Database operations
- API endpoints
- Authentication flow
- Property management flow
