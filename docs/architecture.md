# Architecture Documentation

## Project Structure

### Backend (.NET Core API)
```
RealEstate.API/
├── Controllers/           # API endpoints
├── Models/               # Data models
├── Services/             # Business logic
├── Data/                 # Database context and migrations
├── Middleware/           # Custom middleware
├── Extensions/           # Extension methods
└── Properties/           # Project properties

RealEstate.Tests/
├── Controllers/          # Controller tests
├── Services/             # Service tests
└── TestHelpers/          # Test utilities
```

### Frontend (React/TypeScript)
```
real-estate-client/
├── src/
│   ├── components/       # Reusable UI components
│   ├── pages/           # Page components
│   ├── services/        # API services
│   ├── hooks/           # Custom React hooks
│   ├── context/         # React context
│   ├── types/           # TypeScript types
│   └── utils/           # Utility functions
└── public/              # Static assets
```

## Technology Stack

### Backend
- **.NET 7.0**: Core framework
- **ASP.NET Core**: Web API framework
- **MongoDB.Driver**: MongoDB database driver
- **JWT**: Authentication
- **xUnit**: Testing framework
- **Moq**: Mocking framework

### Frontend
- **React 18**: UI library
- **TypeScript**: Programming language
- **React Router**: Routing
- **Axios**: HTTP client
- **Material-UI**: UI components
- **React Query**: Data fetching
- **Jest**: Testing framework

### Database
- **MongoDB 6.0**: Primary database
  - Collections:
    - Users
    - Properties
    - Favorites

## Architecture Overview

### Backend Architecture
The backend follows a layered architecture:

1. **Controllers Layer**
   - Handles HTTP requests
   - Input validation
   - Route handling
   - Authentication/Authorization

2. **Service Layer**
   - Business logic
   - Data processing
   - External service integration

3. **Data Layer**
   - Database operations
   - Data models
   - MongoDB context

### Frontend Architecture
The frontend follows a component-based architecture:

1. **Components**
   - Reusable UI components
   - Page components
   - Layout components

2. **Services**
   - API integration
   - Data fetching
   - State management

3. **Context**
   - Global state
   - Authentication state
   - Theme management

## Security

### Authentication
- JWT-based authentication
- Token expiration
- Refresh token mechanism

### Authorization
- Role-based access control
- Route protection
- API endpoint protection

### Data Security
- Password hashing
- HTTPS
- Input validation
- MongoDB security best practices

## Performance

### Backend Optimizations
- Async/await operations
- MongoDB indexing
- Response caching
- Compression

### Frontend Optimizations
- Code splitting
- Lazy loading
- Image optimization
- Caching strategies

## Monitoring and Logging

### Backend Logging
- Application logs
- Error tracking
- Performance metrics
- Request/Response logging

### Frontend Monitoring
- Error boundaries
- Performance monitoring
- User analytics
- Console logging

## Testing Strategy

### Backend Testing
- Unit tests
- Integration tests
- Controller tests
- Service tests

### Frontend Testing
- Component tests
- Integration tests
- E2E tests
- Hook tests

## Deployment

### Backend Deployment
- Docker containerization
- Azure App Service
- CI/CD pipeline
- Environment configuration

### Frontend Deployment
- Static hosting
- CDN integration
- Build optimization
- Environment variables
