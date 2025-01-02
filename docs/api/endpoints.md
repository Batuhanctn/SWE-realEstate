# API Endpoints

## Authentication

### Register User
```http
POST /api/auth/register
```

Request body:
```json
{
  "email": "string",
  "password": "string",
  "firstName": "string",
  "lastName": "string",
  "phoneNumber": "string"
}
```

Response:
```json
{
  "message": "User registered successfully"
}
```

### Login
```http
POST /api/auth/login
```

Request body:
```json
{
  "email": "string",
  "password": "string"
}
```

Response:
```json
{
  "token": "string",
  "user": {
    "id": "string",
    "email": "string",
    "firstName": "string",
    "lastName": "string"
  }
}
```

## Properties

### Get All Properties
```http
GET /api/properties
```

Query parameters:
- `page` (optional): Page number
- `pageSize` (optional): Items per page
- `type` (optional): Property type
- `minPrice` (optional): Minimum price
- `maxPrice` (optional): Maximum price
- `location` (optional): Location search

Response:
```json
{
  "items": [
    {
      "id": "string",
      "title": "string",
      "description": "string",
      "price": 0,
      "location": "string",
      "type": "string",
      "imageUrl": "string"
    }
  ],
  "totalCount": 0,
  "pageCount": 0,
  "currentPage": 0
}
```

### Get Property by ID
```http
GET /api/properties/{id}
```

Response:
```json
{
  "id": "string",
  "title": "string",
  "description": "string",
  "price": 0,
  "location": "string",
  "type": "string",
  "status": "string",
  "bedrooms": 0,
  "bathrooms": 0,
  "size": 0,
  "features": ["string"],
  "imageUrls": ["string"],
  "createdAt": "datetime",
  "updatedAt": "datetime"
}
```

### Create Property
```http
POST /api/properties
```

Request body:
```json
{
  "title": "string",
  "description": "string",
  "price": 0,
  "location": "string",
  "type": "string",
  "status": "string",
  "bedrooms": 0,
  "bathrooms": 0,
  "size": 0,
  "features": ["string"]
}
```

Response:
```json
{
  "id": "string",
  "title": "string",
  "description": "string",
  "price": 0,
  "location": "string",
  "type": "string",
  "status": "string",
  "bedrooms": 0,
  "bathrooms": 0,
  "size": 0,
  "features": ["string"],
  "imageUrls": ["string"],
  "createdAt": "datetime",
  "updatedAt": "datetime"
}
```

### Update Property
```http
PUT /api/properties/{id}
```

Request body: Same as Create Property

Response: 204 No Content

### Delete Property
```http
DELETE /api/properties/{id}
```

Response: 204 No Content

## Favorites

### Get User's Favorites
```http
GET /api/favorites
```

Response:
```json
[
  {
    "id": "string",
    "propertyId": "string",
    "property": {
      "id": "string",
      "title": "string",
      "description": "string",
      "price": 0,
      "location": "string",
      "type": "string",
      "imageUrl": "string"
    },
    "createdAt": "datetime"
  }
]
```

### Add to Favorites
```http
POST /api/favorites
```

Request body:
```json
{
  "propertyId": "string"
}
```

Response:
```json
{
  "id": "string",
  "propertyId": "string",
  "userId": "string",
  "createdAt": "datetime"
}
```

### Remove from Favorites
```http
DELETE /api/favorites/{propertyId}
```

Response: 204 No Content

### Check Favorite Status
```http
GET /api/favorites/check/{propertyId}
```

Response:
```json
{
  "isFavorite": true
}
```
