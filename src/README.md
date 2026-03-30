# FluentCMS.Infrastructure

> Modular infrastructure components for the FluentCMS content management system.

## 📖 About

FluentCMS.Infrastructure is the core infrastructure layer of the FluentCMS system, providing essential services such as event-driven communication, data persistence with Entity Framework, database-backed configuration, centralized logging, and a pluggable architecture. It enables decoupled, scalable, and maintainable development by abstracting common concerns like data access, event handling, and plugin management, allowing the application layer to focus on business logic.

## 🚀 Getting Started

### Prerequisites

* .NET 10.0 or higher

### Installation

```bash
# Add the necessary NuGet packages to your project
dotnet add package FluentCMS.Infrastructure.EventBus
dotnet add package FluentCMS.Infrastructure.Providers
dotnet add package FluentCMS.Infrastructure.Repositories
dotnet add package FluentCMS.Infrastructure.Configuration
dotnet add package FluentCMS.Infrastructure.Logging
dotnet add package FluentCMS.Infrastructure.Plugins
```

Then, configure the services in your startup class:

```csharp
builder.Services.AddEventBus()
    .AddProviders()
    .AddEntityFrameworkRepositories(options =>
    {
        // Configure your database provider (e.g., SQL Server or SQLite)
        options.UseSqlServer(connectionString);
    })
    .AddDatabaseConfiguration()
    .AddLogging(options =>
    {
        // Configure logging
    })
    .AddPlugins();
```