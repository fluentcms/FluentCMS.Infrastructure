# FluentCMS.Infrastructure.Logging

A logging infrastructure library for FluentCMS applications, utilizing Serilog for efficient and structured logging.

## Features

- Structured logging with log levels and contextual information.
- Seamless integration with ASP.NET Core applications.

## Installation

To install the package, run the following command in your .NET project:

```bash
dotnet add package FluentCMS.Infrastructure.Logging
```

## Usage

Add the logging configuration to your application's startup (e.g., in `Program.cs`):

```csharp
using FluentCMS.Infrastructure.Logging;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilog((context, loggerConfiguration) =>
{
    loggerConfiguration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
        .WriteTo.File("path/to/logfile.txt", rollingInterval: RollingInterval.Day);
});

// Build the app
var app = builder.Build();
```

This sets up basic structured logging to console and a rolling file.

## Dependencies

- Serilog.AspNetCore (10.0.0)

## License

This project is licensed under the MIT License.