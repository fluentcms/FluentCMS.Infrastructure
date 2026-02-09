# FluentCMS.Infrastructure.Configuration.EntityFramework

## Description

This package provides Entity Framework-based configuration storage for FluentCMS Infrastructure. It allows storing and retrieving application configuration data dynamically from a database, enabling runtime configuration updates without application restarts.

## Key Features

- **EF-backed Configuration**: Store and manage configuration settings in a database using Entity Framework Core.
- **Automatic Caching**: In-memory caching for improved performance.
- **Periodic Reload**: Configurable automatic reloading of configuration from the database.
- **JSON Deserialization**: Supports nested JSON structures, flattened into key-value pairs for .NET configuration providers.
- **Data Seeding**: Includes data seeders for initializing configuration data.

## Installation

Install the package via NuGet:

```bash
dotnet add package FluentCMS.Infrastructure.Configuration.EntityFramework
```

## Basic Usage

To set up Entity Framework-based configuration in your application, follow these steps:

1. Configure the service in `Program.cs` or your startup code:

```csharp
using FluentCMS.Infrastructure.Configuration.EntityFramework;

// In your Program.cs or Startup.cs
builder.Configuration.AddDatabaseConfiguration(options =>
{
    options.UseSqlite("Data Source=configurations.db"); // or other EF provider
    options.ReloadOnChange = TimeSpan.FromMinutes(5); // optional auto-reload
});

// For database migration/seeding
builder.Services.AddFluentCmsInfrastructure(options =>
{
    options.AddDatabaseConfiguration();
});
```

2. The configuration will be loaded from the database and flattened for use with `IConfiguration`.

Note: Ensure the database is created and seeded appropriately using the provided migrations and seeders.

## Dependencies

- Microsoft.Extensions.Configuration (10.0.2)
- Microsoft.Extensions.Configuration.EnvironmentVariables (10.0.2)
- Microsoft.Extensions.Configuration.Json (10.0.2)
- Microsoft.EntityFrameworkCore.Sqlite (10.0.2)
- FluentCMS.Infrastructure.Repositories.EntityFramework
- FluentCMS.Infrastructure.Configuration

## License

This project is licensed under the MIT License.