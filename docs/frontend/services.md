# Services

## API Services

### AuthService
```typescript
class AuthService {
    login(email: string, password: string): Promise<User>;
    register(user: UserRegistration): Promise<User>;
    logout(): void;
    getCurrentUser(): User | null;
}
```

### PropertyService
```typescript
class PropertyService {
    getProperties(filters?: PropertyFilters): Promise<Property[]>;
    getProperty(id: string): Promise<Property>;
    createProperty(property: PropertyCreate): Promise<Property>;
    updateProperty(id: string, property: PropertyUpdate): Promise<Property>;
    deleteProperty(id: string): Promise<void>;
}
```

### FavoriteService
```typescript
class FavoriteService {
    getFavorites(): Promise<Property[]>;
    addFavorite(propertyId: string): Promise<void>;
    removeFavorite(propertyId: string): Promise<void>;
    isFavorite(propertyId: string): Promise<boolean>;
}
```

## Helper Services

### StorageService
Manages local storage for user data and preferences.

### NotificationService
Handles toast notifications and alerts.

### ValidationService
Form validation and error handling.
