# Models

## User

```csharp
public class User
{
    public string Id { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PasswordHash { get; set; }
}
```

## Property

```csharp
public class Property
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public string Location { get; set; }
    public string OwnerId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

## Favorite

```csharp
public class Favorite
{
    public string Id { get; set; }
    public string UserId { get; set; }
    public string PropertyId { get; set; }
    public DateTime CreatedAt { get; set; }
}
```
