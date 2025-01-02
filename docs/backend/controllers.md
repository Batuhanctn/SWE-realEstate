# Controllers

## AuthController

The `AuthController` handles user authentication and authorization:

- `POST /api/auth/register`: Register a new user
- `POST /api/auth/login`: Authenticate a user and generate JWT token

## PropertyController

The `PropertyController` manages property-related operations:

- `GET /api/properties`: Get all properties with optional filtering
- `POST /api/properties`: Create a new property
- `GET /api/properties/{id}`: Get a specific property
- `PUT /api/properties/{id}`: Update a property
- `DELETE /api/properties/{id}`: Delete a property

## FavoriteController

The `FavoriteController` manages user's favorite properties:

- `GET /api/favorites`: Get user's favorite properties
- `POST /api/favorites`: Add a property to favorites
- `DELETE /api/favorites/{id}`: Remove a property from favorites
