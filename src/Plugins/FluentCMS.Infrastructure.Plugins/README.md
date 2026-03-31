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
    // Patterns used to scan for plugin assemblies (default: ["FluentCMS.Plugins.*"])
    options.ScanAssemblyPatterns = ["MyApp.Plugins.*"];

    // If true, plugin loading failures are logged but don't stop application startup
    options.IgnoreErrors = true;

    // Provide a logger factory (required; defaults to NullLoggerFactory)
    options.LoggerFactory = LoggerFactory.Create(b => b.AddConsole());
});

var app = builder.Build();

// Start plugin system (calls Configure on every loaded plugin)
app.UsePluginSystem();
```

### Creating a Plugin

Reference `FluentCMS.Infrastructure.Plugins.Abstractions` from your plugin project and implement the `IPluginStartup` interface:

```csharp
using FluentCMS.Infrastructure.Plugins.Abstractions;

[Plugin] // Marks this class as a plugin entry point for automatic discovery
public class MyPlugin : IPluginStartup
{
    public void ConfigureServices(IServiceCollection services, IConfiguration? configuration)
    {
        // Register plugin services here
    }

    public void Configure(IApplicationBuilder app)
    {
        // Configure middleware or endpoints here
    }
}
```

Name your plugin assembly so it matches one of the `ScanAssemblyPatterns` (e.g. `MyApp.Plugins.MyFeature.dll`). The plugin system discovers and loads all matching assemblies found alongside the host application's executable.

> **Note:** `PluginDiscovery`, `PluginLoader`, and `PluginInitializer` are **internal** implementation details and cannot be used directly. All public interaction with the plugin system goes through `AddPluginSystem()` / `UsePluginSystem()` and the `IPluginManager` service.

## Dependencies

- `Microsoft.Extensions.Logging.Abstractions` (Version 10.0.2)
- `System.Reflection.MetadataLoadContext` (Version 10.0.2)

Target framework: .NET 10.0

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.