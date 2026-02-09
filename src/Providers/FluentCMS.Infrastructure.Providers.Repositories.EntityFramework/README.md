# FluentCMS.Providers.Repositories.EntityFramework

Entity Framework repository providers for FluentCMS Infrastructure.

## Key Features

- EF-based data access providers

## Installation

```bash
dotnet add package FluentCMS.Providers.Repositories.EntityFramework
```

## Usage

To set up the repository provider, register the Entity Framework provider in your dependency injection container:

```csharp
services.AddDbContext<ProviderDbContext>(options =>
    options.UseSqlServer(connectionString)); // or another EF provider

services.AddFluentCMSProviders()
       .UseEntityFrameworkRepositories();
```

## Dependencies

- Entity Framework Core

## License

This project is licensed under the MIT License.
