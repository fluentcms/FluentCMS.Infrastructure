# FluentCMS Infrastructure Plugins Abstractions

This package provides the abstractions and interfaces for the plugin system in FluentCMS Infrastructure. It allows developers to create and manage plugins within the FluentCMS ecosystem.

## Key Features

- `IPluginStartup`: Interface for plugin startup classes, defining methods for configuring services and application pipeline.
- `PluginAttribute`: Attribute to mark classes as plugin startup entries for automatic discovery.

## Installation

Install the package via NuGet:

```bash
dotnet add package FluentCMS.Infrastructure.Plugins.Abstractions
```

## Usage

To create a plugin, implement the `IPluginStartup` interface and mark your class with the `Plugin` attribute.

```csharp
using FluentCMS.Infrastructure.Plugins.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[Plugin] // This attribute marks the class as a plugin
public class SamplePlugin : IPluginStartup
{
    public void ConfigureServices(IServiceCollection services, IConfiguration? configuration)
    {
        // Register services here
    }

    public void Configure(IApplicationBuilder app)
    {
        // Configure middleware here
    }
}
```

## Dependencies

None external.

## License

MIT