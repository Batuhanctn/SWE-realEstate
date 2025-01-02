# Frontend Documentation

This documentation is automatically generated from JSDoc comments in the source code using JSDoc and better-docs.

## Viewing the Documentation

The documentation is generated as a static website. You can view it by:

1. Opening `index.html` in your browser
2. Serving it using a local server:
```bash
npx serve docs/frontend
```

## Updating the Documentation

The documentation is automatically generated from JSDoc comments in the source code. To update it:

1. Make sure your code has proper JSDoc comments
2. Run the documentation generator:
```bash
npm run docs
```

## Documentation Structure

The documentation is organized into several sections:

### Components
- React components with their props and methods
- Usage examples
- Component hierarchy

### Services
- API service functions
- Data transformation utilities
- Authentication services

### Hooks
- Custom React hooks
- Usage examples
- Return values and parameters

### Types
- TypeScript interfaces
- Type definitions
- Enums and constants

## Writing Documentation

### Component Documentation Example
```typescript
/**
 * Property card component that displays property information
 * @component
 * @example
 * ```tsx
 * <PropertyCard
 *   property={{
 *     id: '1',
 *     title: 'Beautiful House',
 *     price: 250000,
 *     location: 'New York'
 *   }}
 *   onFavorite={() => {}}
 * />
 * ```
 */
interface PropertyCardProps {
  /** The property to display */
  property: Property;
  /** Callback when favorite button is clicked */
  onFavorite?: (id: string) => void;
}
```

### Service Documentation Example
```typescript
/**
 * Fetches properties from the API
 * @async
 * @function
 * @param {Object} params - Query parameters
 * @param {number} [params.page=1] - Page number
 * @param {number} [params.pageSize=10] - Items per page
 * @returns {Promise<Property[]>} List of properties
 * @throws {Error} When API request fails
 */
async function getProperties(params: QueryParams): Promise<Property[]>
```

## Best Practices

1. Always include:
   - Component description
   - Props documentation
   - Usage examples
   - Return values
   - Error cases

2. Use TypeScript interfaces for better type documentation

3. Include practical examples in your documentation

4. Document both success and error scenarios

5. Keep documentation up to date with code changes
