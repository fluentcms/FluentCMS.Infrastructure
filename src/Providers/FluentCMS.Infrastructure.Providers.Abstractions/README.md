# FluentCMS.Infrastructure.Providers.Abstractions

[![NuGet Version](https://img.shields.io/nuget/v/FluentCMS.Infrastructure.Providers.Abstractions)](https://www.nuget.org/packages/FluentCMS.Infrastructure.Providers.Abstractions/)

## Description

FluentCMS Infrastructure Providers Abstractions provides fundamental abstractions and interfaces for implementing provider modules in the FluentCMS infrastructure. This library defines the core contracts and base classes that enable a consistent and extensible provider architecture within the FluentCMS ecosystem.

## Key Features

- **IProvider**: Base marker interface that all provider implementations must inherit.
- **IProviderModule**: Defines the core interface for provider modules, including service configuration methods.
- **ProviderModuleBase**: Abstract base classes providing default implementations for provider modules, supporting both typed and untyped providers.
- Built on .NET 10.0 for modern asynchronous and dependency injection patterns.

## Installation

Install the package via NuGet Package Manager:

```bash
dotnet add package FluentCMS.Infrastructure.Providers.Abstractions
```

## Basic Usage

To implement a custom provider, inherit from the provided interfaces and base classes:

```csharp
using FluentCMS.Infrastructure.Providers.Abstractions;
using Microsoft.Extensions.DependencyInjection;

// Example provider interface
public interface IMyProvider : IProvider
{
    Task DoSomethingAsync();
}

// Example provider implementation
public class MyProvider : IMyProvider
{
    public Task DoSomethingAsync() => Task.CompletedTask;
}

// Example provider module
public class MyProviderModule : ProviderModuleBase<IMyProvider, MyProviderOptions>
{
    public override void ConfigureServices(IServiceCollection services, MyProviderOptions options)
    {
        services.AddScoped<IMyProvider, MyProvider>();
    }
}

// Provider options
public class MyProviderOptions
{
    // Define your options here
}
```

Integrate the provider module into your application by registering it with the dependency injection container:

```csharp
var options = new MyProviderOptions();
var module = new MyProviderModule();
var services = new ServiceCollection();
module.ConfigureServices(services, options);
```

## Dependencies

- Microsoft.Extensions.DependencyInjection.Abstractions (>= 10.0.2)
- .NET 10.0

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.