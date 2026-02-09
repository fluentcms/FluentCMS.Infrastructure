# FluentCMS.Infrastructure.Repositories.EntityFramework

This package provides Entity Framework implementations of repositories for the FluentCMS Infrastructure, enabling seamless integration of EF-based data access across the FluentCMS ecosystem.

## Features

- **EF-Based CRUD Operations**: Comprehensive Create, Read, Update, and Delete operations using Entity Framework.
- **Query Building**: Flexible query specifications with LINQ support for filtering, ordering, and pagination.
- **Entity Tracking**: Automatic management of entity state changes during database operations.
- **Data Seeding**: Base classes and services for initializing database data.
- **Schema Validation**: Tools for validating and creating database schemas.
- **Interceptors**: Custom EF interceptors for handling domain events and auditable entities.
- **Exception Handling**: Specialized exceptions, including `EntityNotFoundException`, for robust error management.

## Installation

Install the package via NuGet:

```bash
dotnet add package FluentCMS.Infrastructure.Repositories.EntityFramework
```

## Usage

### Basic Repository Usage

```csharp
using FluentCMS.Infrastructure.Repositories.EntityFramework;

// Assuming you have a DbContext and an entity
public class MyEntityRepository : Repository<MyEntity>
{
    public MyEntityRepository(MyDbContext context) : base(context) { }
}

// Using the repository
var repository = new MyEntityRepository(dbContext);

// Create
await repository.AddAsync(new MyEntity { /* properties */ });

// Read
var entity = await repository.GetByIdAsync(id);

// Update
entity.Property = "new value";
await repository.UpdateAsync(entity);

// Delete
await repository.DeleteAsync(entity);

// Query with specification
var spec = new QuerySpecification<MyEntity>()
    .Where(e => e.IsActive)
    .OrderBy(e => e.Name)
    .Take(10);

var results = await repository.GetAsync(spec);
```

### Configuration Setup

Configure the repository services in your `Program.cs` or `Startup.cs`:

```csharp
builder.Services.AddFluentCmsEntityFramework(options =>
{
    options.AddDatabase<MyDbContext>("ConnectionString", DatabaseProvider.SqlServer);
});
```

## Dependencies

- Entity Framework Core 10.0.2
- FluentCMS.Common
- FluentCMS.Repositories

## License

This project is licensed under the MIT License.