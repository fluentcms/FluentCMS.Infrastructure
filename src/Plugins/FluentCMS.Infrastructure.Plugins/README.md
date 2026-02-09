# FluentCMS.Infrastructure.Plugins

A lightweight and extensible plugin system implementation for FluentCMS Infrastructure, enabling dynamic discovery, loading, and initialization of plugins in .NET applications.

## Features

- **Dynamic Plugin Loading**: Automatically scans and loads plugin assemblies at runtime.
- **Plugin Discovery**: Discovers plugins marked with custom attributes in specified directories.
- **Initialization Management**: Handles plugin initialization with error handling and logging.
- **Assembly Isolation**: Uses custom load contexts for plugin assemblies to prevent conflicts.
- **Configuration Options**: Flexible configuration through `PluginSystemOptions`.

## Installation

Install the package via NuGet:

```bash
dotnet add package FluentCMS.Infrastructure.Plugins
```

## Usage

### Basic Setup

Add the plugin system to your services in `Program.cs`:

```csharp
using FluentCMS.Infrastructure.Plugins;

var builder = WebApplication.CreateBuilder(args);

// Configure plugin system
builder.Services.AddPluginSystem(builder.Configuration, options =>
{
    options.PluginDirectory = "path/to/plugins";
    // Additional configuration...
});

var app = builder.Build();

// Start plugin system
app.UsePluginSystem();
```

### Creating a Plugin

Implement the `IPluginStartup` interface in your plugin assembly:

```csharp
using FluentCMS.Infrastructure.Plugins.Abstractions;

[PluginAttribute]
public class MyPlugin : IPluginStartup
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // Add plugin services here
    }

    public void Configure(IApplicationBuilder app)
    {
        // Configure the application pipeline here
    }
}
```

Place your plugin assembly in the specified plugin directory, and it will be loaded automatically.

## Dependencies

- `Microsoft.Extensions.Logging.Abstractions` (Version 10.0.2)
- `System.Reflection.MetadataLoadContext` (Version 10.0.2)

Target framework: .NET 10.0

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.