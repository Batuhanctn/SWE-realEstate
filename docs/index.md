# RealEstate Application Documentation

Welcome to the comprehensive documentation for the RealEstate application. This documentation covers all aspects of the application including the backend API, frontend client, and test suite.

## Components

### Backend API
The backend API is built with ASP.NET Core and provides:
- User authentication and authorization
- Property management
- Favorite properties functionality
- Search and filtering capabilities

[Learn more about the Backend API](backend/index.md)

### Frontend Client
The React-based frontend client offers:
- User-friendly interface
- Property listing and details
- User profile management
- Favorite properties management

[Learn more about the Frontend Client](frontend/index.md)

### Test Suite
Comprehensive test coverage including:
- Unit tests for API controllers
- Integration tests
- Frontend component tests

[Learn more about the Test Suite](tests/index.md)

## Getting Started

1. Clone the repository
2. Set up the backend:
   ```bash
   cd RealEstate.API
   dotnet run
   ```

3. Set up the frontend:
   ```bash
   cd real-estate-client
   npm install
   npm start
   ```

4. Run the tests:
   ```bash
   cd RealEstate.Tests
   dotnet test
   ```

## Architecture Overview

The application follows a modern web architecture:
- Backend: ASP.NET Core Web API
- Frontend: React with TypeScript
- Database: MongoDB
- Authentication: JWT-based authentication

## Contributing

Please read our contributing guidelines before submitting pull requests.
