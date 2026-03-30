# FluentCMS.Infrastructure.Configuration.EntityFramework.SqlServer

A SQL Server-specific Entity Framework configuration provider for the FluentCMS Infrastructure. This package enables using SQL Server as the backend for storing and retrieving application configuration data via Entity Framework.

## Key Features

- SQL Server database provider integration for FluentCMS configuration.
- Extension methods to easily set up SQL Server-backed configuration in .NET applications.
- Leverages Entity Framework Core for ORM-based configuration storage and retrieval.
- Supports automatic configuration reloading with optional intervals.

## Installation

Install the package via NuGet Package Manager:

```
dotnet add package FluentCMS.Infrastructure.Configuration.EntityFramework.SqlServer --version 1.0.0
```

## Usage

To use SQL Server for configuration, add the SQL Server configuration extension to your `IConfigurationBuilder`:

```csharp
using FluentCMS.Configuration.EntityFramework.SqlServer;
using Microsoft.Extensions.Configuration;

var builder = new ConfigurationBuilder()
    .AddSqlServerConfiguration("Server=your-server;Database=your-db;Trusted_Connection=True;");

// Build the configuration
var configuration = builder.Build();
```

Replace `"Server=your-server;Database=your-db;Trusted_Connection=True;"` with your actual SQL Server connection string.

For configuration with automatic reloading, specify a reload interval:

```csharp
builder.AddSqlServerConfiguration(connectionString, TimeSpan.FromSeconds(30));
```

## Dependencies

This package depends on:

- `Microsoft.EntityFrameworkCore.SqlServer` (version 10.0.2)
- `FluentCMS.Infrastructure.Configuration.EntityFramework` (project reference, included as a dependency in NuGet package)

Ensure your project targets .NET 10.0 or later.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.