# FluentCMS Infrastructure Configuration

## Overview

FluentCMS Infrastructure Configuration provides a robust solution for managing configuration settings in FluentCMS applications. This library enables database-backed configuration options, allowing dynamic storage and retrieval of application settings through the ASP.NET Core Options pattern. It integrates seamlessly with configuration repositories to persist settings in the database, facilitating flexible and scalable configuration management.

## Key Features

- **Database-Backed Options**: Store and manage configuration options in a database using the ASP.NET Core Options pattern.
- **Dynamic Configuration Management**: Register configuration sections for database storage and bind them to configuration sources.
- **Thread-Safe Registry**: Singleton registry ensures thread-safe access and prevents duplicate registrations.
- **Extension Methods**: Fluent API for registering options classes with optional configuration binding.

## Installation

Install the NuGet package:

```bash
dotnet add package FluentCMS.Infrastructure.Configuration
```

## Usage

### Basic Setup

First, add the database configuration registry to your services:

```csharp
using FluentCMS.Infrastructure.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Register the database configuration registry
builder.Services.AddDatabaseConfigurationRegistry();

// Optional: Add your configuration repository dependency
// builder.Services.AddScoped<IConfigurationRepository, YourConfigurationRepository>();
```

### Configuring Options

Register options classes for database storage:

```csharp
// Register options class for database storage without binding
builder.Services.AddDbOptions<MyOptions>("MySection");

// Register and bind to configuration section
builder.Services.AddDbOptions<MyOptions>("MySection", builder.Configuration);

// With custom binder options
builder.Services.AddDbOptions<MyOptions>("MySection", builder.Configuration, binderOptions =>
{
    binderOptions.BindNonPublicProperties = false;
    // Configure other binder options as needed
});
```

### Example Options Class

```csharp
public class MyOptions
{
    public string Setting1 { get; set; } = string.Empty;
    public int Setting2 { get; set; }
}
```

### Using Options

Inject and use the configured options in your services:

```csharp
public class MyService(IOptions<MyOptions> options)
{
    private readonly MyOptions _options = options.Value;

    public void DoSomething()
    {
        Console.WriteLine(_options.Setting1);
    }
}
```

## Dependencies

- Microsoft.Extensions.Options.ConfigurationExtensions (10.0.2)

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.