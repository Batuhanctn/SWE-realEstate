# Backend API Documentation

The RealEstate.API is built with ASP.NET Core and provides a RESTful API for the real estate application.

## Features

- User authentication and authorization
- Property management
- Favorite properties
- Search and filtering
- Image upload and management

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

## Architecture

The backend follows a clean architecture pattern:
- Controllers for handling HTTP requests
- Services for business logic
- Repositories for data access
- Models for data representation

## Technologies

- ASP.NET Core 8.0
- MongoDB
- JWT Authentication
- xUnit for testing
- Swagger/OpenAPI for API documentation
