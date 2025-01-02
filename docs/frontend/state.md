# State Management

## Context Providers

### AuthContext
```typescript
interface AuthContext {
    user: User | null;
    login: (email: string, password: string) => Promise<void>;
    logout: () => void;
    isAuthenticated: boolean;
}
```

### PropertyContext
```typescript
interface PropertyContext {
    properties: Property[];
    loading: boolean;
    error: Error | null;
    fetchProperties: (filters?: PropertyFilters) => Promise<void>;
    createProperty: (property: PropertyCreate) => Promise<void>;
    updateProperty: (id: string, property: PropertyUpdate) => Promise<void>;
    deleteProperty: (id: string) => Promise<void>;
}
```

### FavoriteContext
```typescript
interface FavoriteContext {
    favorites: Property[];
    loading: boolean;
    error: Error | null;
    addFavorite: (propertyId: string) => Promise<void>;
    removeFavorite: (propertyId: string) => Promise<void>;
    isFavorite: (propertyId: string) => boolean;
}
```

## Custom Hooks

### useAuth
```typescript
const useAuth = () => {
    const context = useContext(AuthContext);
    if (!context) {
        throw new Error('useAuth must be used within an AuthProvider');
    }
    return context;
};
```

### useProperty
```typescript
const useProperty = () => {
    const context = useContext(PropertyContext);
    if (!context) {
        throw new Error('useProperty must be used within a PropertyProvider');
    }
    return context;
};
```

### useFavorite
```typescript
const useFavorite = () => {
    const context = useContext(FavoriteContext);
    if (!context) {
        throw new Error('useFavorite must be used within a FavoriteProvider');
    }
    return context;
};
```
