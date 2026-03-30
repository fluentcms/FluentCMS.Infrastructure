# Plugins

> A modular plugin system for FluentCMS infrastructure, enabling dynamic discovery, loading, and initialization of plugins to extend core functionality.

## 📖 About
The Plugins project is a collection of libraries that enable a flexible plugin architecture for FluentCMS. It provides mechanisms for discovering plugins from directories, loading them in isolated AssemblyLoadContexts to prevent memory leaks, initializing plugin instances, and managing their lifecycles. This allows developers to extend the CMS capabilities dynamically without modifying the core codebase, ensuring modularity, safety, and ease of integration.

## 🚀 Getting Started

### Prerequisites
* .NET 10.0

### Installation
```bash
# Build the projects
dotnet build FluentCMS.Infrastructure.Plugins.Abstractions.csproj
dotnet build FluentCMS.Infrastructure.Plugins.csproj
```

Since this project consists of multiple .NET libraries, build them and reference the resulting assemblies in your application.

### Setup
Configure the plugin system in your application:

```csharp
using FluentCMS.Infrastructure.Plugins;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddPluginSystem(options =>
{
    // Configure options, e.g.
    options.PluginDirectories = ["path/to/plugins"];
});
var app = builder.Build();
app.UsePluginSystem();
```

### Usage Examples
To create a plugin, implement the `IPluginStartup` interface from `FluentCMS.Infrastructure.Plugins.Abstractions`, and mark the class with the `Plugin` attribute.

Example plugin:
```csharp
using FluentCMS.Infrastructure.Plugins.Abstractions;

[Plugin]
public class MyPlugin : IPluginStartup
{
    public void ConfigureServices(IServiceCollection services, IConfiguration? configuration)
    {
        // Register plugin-specific services
        services.AddScoped<IMyPluginService, MyPluginService>();
    }

    public void Configure(IApplicationBuilder app)
    {
        // Configure middleware or routes
        app.MapGet("/myplugin", () => "Hello from Plugin!");
    }

    // Optionally override Name, Version, Priorities
    public override string Name => "My Custom Plugin";
}
```

Build this as a separate assembly and place it in a directory configured in `PluginDirectories`.

The system will discover, load, initialize, and start the plugin automatically.

## 📦 Key Features
- **Dynamic Discovery**: Scans directories for plugin assemblies based on file patterns.
- **Isolated Loading**: Uses custom `AssemblyLoadContext` to load plugins collectibly, preventing memory leaks and allowing unloading.
- **Initialization and Lifecycle**: Handles plugin instantiation, service registration, and application configuration with priority ordering.
- **Error Handling**: Provides robust exception handling, logging, and optional error ignoring for resilient operation.
- **Abstraction-Based Design**: Extensible through interfaces like `IPluginDiscovery`, `IPluginLoader`, `IPluginInitializer`, and `IPluginManager`.
- **Configuration Scoping**: Each plugin receives its own configuration section under 'Plugins:{PluginName}'.

## 📚 Dependencies
Key dependencies include:
- .NET 10.0
- Microsoft.AspNetCore.Http.Abstractions (2.3.9)
- Microsoft.Extensions.Configuration.Abstractions (10.0.2)
- Microsoft.Extensions.DependencyInjection.Abstractions (10.0.2)
- Microsoft.Extensions.Logging.Abstractions (10.0.2)
- System.Reflection.MetadataLoadContext (10.0.2)