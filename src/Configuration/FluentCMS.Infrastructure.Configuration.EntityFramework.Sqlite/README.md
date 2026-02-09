# FluentCMS.Infrastructure.Configuration.EntityFramework.Sqlite

This package provides SQLite-specific Entity Framework configuration for the FluentCMS Infrastructure layer. It enables storing application configuration in an SQLite database using Entity Framework as the ORM, extending the base FluentCMS configuration capabilities to support SQLite as a lightweight, file-based database option.

## Key Features

- SQLite provider for storing and retrieving application configuration.
- Seamless integration with Entity Framework and the FluentCMS configuration system.
- Support for automatic configuration reloading based on a specified interval.

## Installation

Install this package via NuGet:

```bash
dotnet add package FluentCMS.Infrastructure.Configuration.EntityFramework.Sqlite
```

## Usage

To use SQLite for configuration storage, add the SQLite configuration provider to your configuration builder. Ensure you have the connection string defined in your appsettings.json or environment variables.

```csharp
using FluentCMS.Infrastructure.Configuration.EntityFramework.Sqlite;

var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddSqliteConfiguration("DefaultConnection", TimeSpan.FromSeconds(30)); // Reload every 30 seconds
```

Make sure to register the required services and configure the database context as needed in your application. This extension method relies on the base Entity Framework configuration from `FluentCMS.Infrastructure.Configuration.EntityFramework`.

## Dependencies

- Microsoft.EntityFrameworkCore.Sqlite (latest stable version)
- FluentCMS.Infrastructure.Configuration.EntityFramework

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.