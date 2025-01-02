# Getting Started with Real Estate API

## Prerequisites
- .NET 7.0 SDK
- MongoDB 6.0
- Visual Studio 2022 or VS Code

## Setup

1. Clone the repository:
```bash
git clone https://github.com/yourusername/real-estate-api.git
```

2. Restore dependencies:
```bash
dotnet restore
```

3. Update database connection in appsettings.json:
```json
{
  "MongoDb": {
    "ConnectionString": "your_connection_string",
    "DatabaseName": "RealEstate"
  }
}
```

4. Run the application:
```bash
dotnet run
```

## Project Structure

```
RealEstate.API/
├── Controllers/     # API endpoints
├── Models/         # Data models
├── Services/       # Business logic
└── Data/          # Database context
```

## Authentication

The API uses JWT for authentication. Include the token in requests:

```http
Authorization: Bearer your_jwt_token
```

## API Usage Examples

### Register User
```http
POST /api/auth/register
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "password123",
  "firstName": "John",
  "lastName": "Doe"
}
```

### Create Property
```http
POST /api/properties
Content-Type: application/json
Authorization: Bearer your_jwt_token

{
  "title": "Beautiful House",
  "price": 250000,
  "location": "New York"
}
```
