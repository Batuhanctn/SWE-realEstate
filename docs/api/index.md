# API Reference

This section contains the complete API reference documentation for the RealEstate application.

## Namespaces

### RealEstate.API
Contains the core API implementation:
- Controllers
- Models
- Services
- Data access layer

### RealEstate.Tests
Contains the test implementation:
- Controller tests
- Service tests
- Integration tests

## Common Patterns

### Controllers
All API controllers follow RESTful principles:
- GET for retrieving data
- POST for creating new resources
- PUT for updating existing resources
- DELETE for removing resources

### Authentication
JWT-based authentication is used throughout the API:
- Token generation on login
- Token validation middleware
- Authorized endpoints

### Error Handling
Consistent error handling across the API:
- HTTP status codes
- Error messages
- Validation responses

## API Endpoints

### Authentication
- POST /api/auth/register
- POST /api/auth/login

### Properties
- GET /api/properties
- POST /api/properties
- GET /api/properties/{id}
- PUT /api/properties/{id}
- DELETE /api/properties/{id}

### Favorites
- GET /api/favorites
- POST /api/favorites
- DELETE /api/favorites/{id}

## Models

### User
```csharp
public class User
{
    public string Id { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PasswordHash { get; set; }
}
```

### Property
```csharp
public class Property
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public string Location { get; set; }
}
```
