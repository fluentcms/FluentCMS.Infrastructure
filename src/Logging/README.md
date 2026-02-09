# Logging

> Centralized logging module for FluentCMS using Serilog

## 📖 About
This project provides a centralized logging infrastructure for the FluentCMS application. It uses Serilog to configure structured logging with output to the console and daily rolling log files, facilitating efficient monitoring, debugging, and troubleshooting of application behavior.

## ✨ Key Features
- Structured logging with Serilog
- Console output for development
- Daily rolling log files for persistence
- Configurable minimum log levels (Debug, with override for Microsoft to Warning)
- Integration with .NET's dependency injection via ILoggerFactory

## 🚀 Getting Started

### Prerequisites
* .NET 8 or later

### Installation
```bash
# Install dependencies
dotnet add package Serilog
dotnet add package Serilog.Extensions.Logging
dotnet add package Serilog.Console
dotnet add package Serilog.File
```

### Integration
In your application's `IHostBuilder` setup (e.g., in `Program.cs`):

```csharp
using FluentCMS.Infrastructure.Logging;

var builder = WebApplication.CreateBuilder(args);

// Initialize the logger factory
var loggerFactory = builder.Host.InitLogFactory();

// Now you can use it for DI or pass to services
builder.Services.AddSingleton(loggerFactory);
```

Logs will be written to the console and to daily files in the `logs/` directory (e.g., `ourapp-2023-05-01.log`).