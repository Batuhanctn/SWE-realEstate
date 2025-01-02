# Getting Started

## Prerequisites

### Backend (.NET Core API)
- .NET 7.0 SDK or later
- MongoDB 6.0 or later
- Visual Studio 2022 or Visual Studio Code

### Frontend (React/TypeScript)
- Node.js 18.x or later
- npm 9.x or later
- Visual Studio Code (recommended)

## Installation

### Backend Setup
1. Clone the repository:
```bash
git clone https://github.com/yourusername/real-estate-api.git
```

2. Navigate to the API project:
```bash
cd RealEstate.API
```

3. Install dependencies:
```bash
dotnet restore
```

4. Update the database connection string in `appsettings.json`:
```json
{
  "MongoDb": {
    "ConnectionString": "your_mongodb_connection_string",
    "DatabaseName": "RealEstate"
  }
}
```

5. Run the application:
```bash
dotnet run
```

The API will be available at `https://localhost:7000`

### Frontend Setup
1. Navigate to the client project:
```bash
cd real-estate-client
```

2. Install dependencies:
```bash
npm install
```

3. Update the API URL in `.env`:
```env
REACT_APP_API_URL=https://localhost:7000
```

4. Start the development server:
```bash
npm start
```

The frontend will be available at `http://localhost:3000`

## Configuration

### Backend Configuration
The backend can be configured through `appsettings.json`:

```json
{
  "Jwt": {
    "Key": "your_secret_key",
    "Issuer": "your_issuer",
    "Audience": "your_audience"
  },
  "MongoDb": {
    "ConnectionString": "your_mongodb_connection_string",
    "DatabaseName": "RealEstate"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### Frontend Configuration
The frontend can be configured through environment variables:

```env
REACT_APP_API_URL=https://localhost:7000
REACT_APP_GOOGLE_MAPS_KEY=your_google_maps_api_key
REACT_APP_IMAGE_UPLOAD_MAX_SIZE=5242880
```

## Development Tools

### Recommended VS Code Extensions
- C# Dev Kit
- MongoDB for VS Code
- ESLint
- Prettier
- React Developer Tools

### Debugging
1. Backend:
   - Use Visual Studio's built-in debugger
   - Or use VS Code with C# extension

2. Frontend:
   - Use Chrome DevTools
   - React Developer Tools browser extension
   - VS Code debugger with Chrome/Edge debugging
