# FluentCMS.Infrastructure.Repositories.EntityFramework.Sqlite

This package provides SQLite-specific Entity Framework repositories for the FluentCMS Infrastructure. It enables seamless integration of SQLite databases into FluentCMS applications, offering efficient data access and management capabilities.

## Key Features

- **SQLite Data Access**: Simplified configuration for SQLite database operations using Entity Framework Core.
- **Fluent API**: Easy-to-use extension methods for configuring SQLite database connections.
- **Integration**: Built on top of FluentCMS Infrastructure repositories, promoting consistency across the framework.

## Installation

Install the package via NuGet Package Manager:

```bash
dotnet add package FluentCMS.Infrastructure.Repositories.EntityFramework.Sqlite
```

## Usage

To configure your FluentCMS application to use SQLite, use the `UseSqlite` extension method on the `DatabaseConfigurationBuilder`:

```csharp
using FluentCMS.Infrastructure.Repositories.EntityFramework.Sqlite;

// Assuming you have a DatabaseConfigurationBuilder instance
var builder = new DatabaseConfigurationBuilder();

builder.UseSqlite("Data Source=mydatabase.db");
```

This sets up the Entity Framework context to use SQLite with the specified connection string.

## Dependencies

- [.NET 10.0](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Microsoft.EntityFrameworkCore.Sqlite](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Sqlite) (>= 10.0.2)
- [FluentCMS.Infrastructure.Repositories.EntityFramework](..\\FluentCMS.Infrastructure.Repositories.EntityFramework)

## License

This project is licensed under the MIT License. See [LICENSE](LICENSE) for more details.