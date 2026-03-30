# FluentCMS.Infrastructure.Repositories

A NuGet package providing data access repositories for FluentCMS Infrastructure.

## Description

FluentCMS.Infrastructure.Repositories is a part of the FluentCMS ecosystem, offering repository abstractions to facilitate data access operations in the infrastructure layer. It supports CRUD operations, query specifications, pagination, database initialization conditions, schema validation, and domain events for entity operations.

## Key Features

- **Repository Abstractions**: Interfaces and implementations for standard data access operations via `IRepository<TEntity>`.
- **Query Specifications**: Fluent API for building complex queries with filtering, ordering, and pagination.
- **Pagination Support**: `PagedResult<T>` for handling paginated data results.
- **Database Initialization**: Conditions and options for seeding and validating database schemas.
- **Domain Events**: Events for tracking entity creation, updates, and deletions.

## Installation

Install the package via NuGet:

```bash
dotnet add package FluentCMS.Infrastructure.Repositories
```

## Usage

Here's a basic example of using the repository interface:

```csharp
using FluentCMS.Infrastructure.Repositories;
using FluentCMS.Infrastructure.Repositories.Abstractions;

// Assume you have a service or class with dependency injection
public class MyService
{
    private readonly IRepository<MyEntity> _repository;

    public MyService(IRepository<MyEntity> repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<MyEntity>> GetAllEntitiesAsync()
    {
        return await _repository.GetAllAsync();
    }
}
```

For more advanced queries using specifications:

```csharp
using FluentCMS.Infrastructure.Repositories.Abstractions;

// Define a query specification
var spec = new QuerySpecification<MyEntity>()
    .Where(e => e.IsActive)
    .OrderByDescending(e => e.CreatedAt)
    .Paginate(page: 1, pageSize: 10);

var result = await _repository.ListAsync(spec);
```

## Dependencies

- [Microsoft.Extensions.Hosting.Abstractions](https://www.nuget.org/packages/Microsoft.Extensions.Hosting.Abstractions/) (10.0.2)

## License

This project is licensed under the MIT License - see the [LICENSE](../LICENSE) file for details.