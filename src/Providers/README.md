# Providers

> A modular provider system for FluentCMS, enabling extensible data access and feature implementation.

## 📖 About

The Providers project is a core component of FluentCMS, providing a structured way to implement and manage pluggable provider modules. It separates abstractions from concrete implementations, utilizing Entity Framework for data persistence and supporting dynamic module discovery and management. This architecture allows for easy extension of CMS functionality through provider modules that can be discovered at runtime, configured, and injected into the application's dependency injection container.

## 🚀 Getting Started

### Prerequisites
* .NET 10.0 or higher
* FluentCMS infrastructure dependencies

### Installation
```bash
# Add package reference (if applicable, adjust path as needed)
dotnet add package FluentCMS.Infrastructure.Providers
```

### Setup
Add provider services to your `IServiceCollection` in `Program.cs`:

```csharp
builder.Services.AddProviders();
```

### Configuration
Configure provider discovery options via `appsettings.json` or directly in code:

```json
{
  "Providers": {
    "EnableLogging": true,
    "AssemblyPrefixes": ["FluentCMS"],
    "IgnoreExceptions": false
  }
}
```

## 🏗️ Architecture Overview

The Providers system is organized into several key components:

### Abstractions Layer
Defines interfaces and base classes for provider modules:
- `IProviderModule<TProvider, TOptions>`: Generic interface for typed provider modules
- `ProviderModuleBase<TProvider, TOptions>`: Base class implementation
- `IProvider`: Marker interface for concrete providers

### Core Implementation
Handles module discovery, caching, and management:
- **ProviderDiscovery**: Scans assemblies for provider modules
- **ProviderManager**: Manages active providers and module retrieval
- **ProviderCatalogCache**: Thread-safe caching of provider catalogs
- **ProviderModuleCatalogCache**: Caches discovered modules

### Data Layer
Entity Framework-based persistence:
- **ProviderRepository**: Repository pattern implementation for CRUD operations
- **ProviderDbContext**: EF Core context for provider entities
- **ProviderSchemaValidator**: Ensures database schema integrity
- **ProviderDataSeeder**: Seeds initial provider data

## ✨ Key Features

- **Dynamic Module Discovery**: Automatically detects and loads provider modules from DLLs at runtime
- **Thread-Safe Operations**: Concurrent dictionary-based caching for high-performance access
- **Dependency Injection Integration**: Seamlessly integrates with Microsoft.Extensions.DependencyInjection
- **Configurable Areas**: Organizes providers by functional areas (e.g., admin, user)
- **Active Provider Management**: Supports single active provider per area to avoid conflicts
- **Entity Framework Support**: Built-in repository pattern with EF Core for data persistence
- **Validation and Seeding**: Schema validation and automated data seeding capabilities

## 📋 Usage

### Creating a Provider Module

Implement `IProviderModule<TProvider, TOptions>` for your custom provider:

```csharp
public class MyProvider : IProvider
{
    // Implementation
}

public class MyProviderOptions
{
    // Configuration options
}

public class MyProviderModule : ProviderModuleBase<MyProvider, MyProviderOptions>
{
    public override void Configure(IServiceCollection services, string? name)
    {
        services.AddSingleton<MyProvider>();
    }
}
```

### Registering and Using Providers

Configure providers in your startup:

```csharp
builder.AddProviders()
    .AddEntityFrameworkProviders();
```

Access providers through `IProviderManager`:

```csharp
public class MyService
{
    private readonly IProviderManager _providerManager;

    public MyService(IProviderManager providerManager)
    {
        _providerManager = providerManager;
    }

    public async Task<MyProvider> GetActiveProvider(string area)
    {
        var activeByArea = await _providerManager.GetActiveByArea(area);
        // Use the active provider
    }
}
```

## 📦 Dependencies

* Microsoft.Extensions.DependencyInjection.Abstractions (10.0.2)
* Microsoft.Extensions.Hosting.Abstractions (10.0.2)
* Microsoft.Extensions.Logging.Console (10.0.2)
* FluentCMS Core Repositories
* Entity Framework Core (via Repositories)