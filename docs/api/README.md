# API Documentation

## Overview
The Real Estate API provides endpoints for managing properties, user authentication, and favorites. The API follows RESTful principles and uses JWT for authentication.

## Base URL
```
https://localhost:7000/api
```

## Authentication
All protected endpoints require a valid JWT token in the Authorization header:
```
Authorization: Bearer {your_jwt_token}
```

## Endpoints Summary

### Authentication
- `POST /auth/register` - Register a new user
- `POST /auth/login` - Authenticate user and get token

### Properties
- `GET /properties` - Get all properties
- `GET /properties/{id}` - Get property by ID
- `POST /properties` - Create new property
- `PUT /properties/{id}` - Update property
- `DELETE /properties/{id}` - Delete property
- `GET /properties/search` - Search properties
- `GET /properties/user/{userId}` - Get user's properties

### Favorites
- `GET /favorites` - Get user's favorites
- `POST /favorites` - Add property to favorites
- `DELETE /favorites/{id}` - Remove from favorites
- `GET /favorites/check/{propertyId}` - Check if property is favorited

## Detailed Documentation
- [Authentication API](./auth.md)
- [Properties API](./properties.md)
- [Favorites API](./favorites.md)

## Error Handling
The API uses standard HTTP status codes:

- 200: Success
- 201: Created
- 400: Bad Request
- 401: Unauthorized
- 403: Forbidden
- 404: Not Found
- 500: Internal Server Error

Error responses follow this format:
```json
{
  "message": "Error description",
  "details": ["Additional error details"]
}
```

## Rate Limiting
API requests are limited to:
- 100 requests per minute for authenticated users
- 20 requests per minute for unauthenticated users

## Data Models

### User
```json
{
  "id": "string",
  "email": "string",
  "firstName": "string",
  "lastName": "string",
  "phoneNumber": "string",
  "createdAt": "datetime",
  "updatedAt": "datetime"
}
```

### Property
```json
{
  "id": "string",
  "userId": "string",
  "title": "string",
  "description": "string",
  "price": "decimal",
  "location": "string",
  "type": "string",
  "status": "string",
  "bedrooms": "integer",
  "bathrooms": "integer",
  "size": "double",
  "features": ["string"],
  "imageUrl": "string",
  "imageUrls": ["string"],
  "createdAt": "datetime",
  "updatedAt": "datetime"
}
```

### Favorite
```json
{
  "id": "string",
  "userId": "string",
  "propertyId": "string",
  "createdAt": "datetime"
}
```
