# Testing Documentation

## Overview
This document outlines the testing strategy and implementation for the Real Estate API project. The project uses xUnit for backend testing and Jest for frontend testing.

## Backend Testing

### Test Structure
```
RealEstate.Tests/
├── Controllers/
│   ├── AuthControllerTests.cs
│   ├── PropertyControllerTests.cs
│   ├── PropertiesControllerTests.cs
│   └── FavoriteControllerTests.cs
├── Services/
│   └── PropertyServiceTests.cs
└── TestHelpers/
    └── MockHelpers.cs
```

### Controller Tests

#### AuthController Tests
Tests authentication operations:
- User registration
- User login
- Input validation
- Error handling

Example:
```csharp
[Fact]
public async Task Register_ValidUser_ReturnsOk()
{
    // Arrange
    var request = new RegisterRequest
    {
        Email = "test@example.com",
        Password = "Password123!",
        FirstName = "John",
        LastName = "Doe"
    };

    // Act
    var result = await _controller.Register(request);

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    var response = Assert.IsType<ApiResponse>(okResult.Value);
    Assert.Equal("User registered successfully", response.Message);
}
```

#### PropertyController Tests
Tests property management operations:
- Get all properties
- Get property by ID
- Create property
- Update property
- Delete property
- Search properties

Example:
```csharp
[Fact]
public async Task GetProperties_ReturnsAllProperties()
{
    // Arrange
    var properties = GetTestProperties();
    _mockPropertyService.Setup(s => s.GetAsync())
        .ReturnsAsync(properties);

    // Act
    var result = await _controller.Get();

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result.Result);
    var returnValue = Assert.IsType<List<Property>>(okResult.Value);
    Assert.Equal(properties.Count, returnValue.Count);
}
```

### Service Tests

#### PropertyService Tests
Tests business logic:
- Database operations
- Data validation
- Error handling
- Business rules

Example:
```csharp
[Fact]
public async Task CreateProperty_ValidProperty_SavesToDatabase()
{
    // Arrange
    var property = new Property
    {
        Title = "Test Property",
        Price = 100000
    };

    // Act
    await _service.CreateAsync(property);

    // Assert
    _mockCollection.Verify(c => c.InsertOneAsync(
        It.IsAny<Property>(),
        It.IsAny<InsertOneOptions>(),
        It.IsAny<CancellationToken>()
    ), Times.Once);
}
```

## Frontend Testing

### Test Structure
```
real-estate-client/
├── src/
│   └── __tests__/
│       ├── components/
│       ├── pages/
│       ├── services/
│       └── hooks/
```

### Component Tests
Tests React components:
- Rendering
- User interactions
- State changes
- Props validation

Example:
```typescript
describe('PropertyCard', () => {
  it('renders property information correctly', () => {
    const property = {
      id: '1',
      title: 'Test Property',
      price: 100000,
      location: 'Test Location'
    };

    render(<PropertyCard property={property} />);
    
    expect(screen.getByText('Test Property')).toBeInTheDocument();
    expect(screen.getByText('$100,000')).toBeInTheDocument();
    expect(screen.getByText('Test Location')).toBeInTheDocument();
  });
});
```

### Service Tests
Tests API services:
- API calls
- Data transformation
- Error handling
- Response parsing

Example:
```typescript
describe('PropertyService', () => {
  it('fetches properties successfully', async () => {
    const mockProperties = [
      { id: '1', title: 'Property 1' },
      { id: '2', title: 'Property 2' }
    ];

    axios.get.mockResolvedValueOnce({ data: mockProperties });

    const result = await PropertyService.getProperties();
    
    expect(result).toEqual(mockProperties);
    expect(axios.get).toHaveBeenCalledWith('/api/properties');
  });
});
```

## Test Coverage
- Backend: > 80% code coverage
- Frontend: > 70% code coverage

### Coverage Reports
Generated using:
- Backend: coverlet
- Frontend: Jest coverage

## Running Tests

### Backend Tests
```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test RealEstate.Tests

# Run with coverage
dotnet test /p:CollectCoverage=true
```

### Frontend Tests
```bash
# Run all tests
npm test

# Run with coverage
npm test -- --coverage

# Run specific test file
npm test -- PropertyCard.test.tsx
```

## Continuous Integration
Tests are run automatically:
- On pull requests
- On merge to main branch
- Nightly builds

### CI Pipeline
1. Build project
2. Run backend tests
3. Run frontend tests
4. Generate coverage reports
5. Deploy if all tests pass
