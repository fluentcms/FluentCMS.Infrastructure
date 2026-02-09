# FluentCMS Infrastructure Providers

FluentCMS Infrastructure Providers is a NuGet package that provides essential service providers for the FluentCMS Infrastructure framework. This package enables the dynamic discovery, management, and configuration of provider modules, facilitating extensible and modular hosting and logging capabilities within FluentCMS applications.

## Features

- **Hosting Providers**: Integrate with Microsoft.Extensions.Hosting for robust application hosting.
- **Logging Providers**: Utilize Microsoft.Extensions.Logging.Console for console-based logging.
- **Dynamic Provider Discovery**: Automatically scan and load provider modules from DLLs in the executable directory.
- **Thread-Safe Caching**: Ensure safe concurrent access to provider catalogs and modules.
- **Configuration Management**: Support for provider configuration via JSON or in-memory settings.

## Installation

Install the package via NuGet Package Manager:

```
dotnet add package FluentCMS.Infrastructure.Providers
```

## Usage

To integrate FluentCMS Infrastructure Providers into your application, configure the services in your `Program.cs` or startup code:

```csharp
using FluentCMS.Infrastructure.Providers;

var builder = WebApplication.CreateBuilder(args);

// Add provider services
builder.Services.AddProviders(options =>
{
    // Configure options as needed
    options.EnableLogging = true;
    options.AssemblyPrefixes = ["FluentCMS.Provider"];
});

// Configure other services...

var app = builder.Build();

// Use the app...
app.Run();
```

This sets up the provider management system, allowing your application to dynamically load and use provider modules for hosting and logging.

## Dependencies

- Microsoft.Extensions.Hosting.Abstractions (v10.0.2)
- Microsoft.Extensions.Logging.Console (v10.0.2)

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.