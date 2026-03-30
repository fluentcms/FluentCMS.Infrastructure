# FluentCMS.Infrastructure.Common

This project provides common utilities and base classes for FluentCMS infrastructure components, enabling consistent entity management, auditing, and user context handling across the FluentCMS ecosystem.

## Key Features

- **Base Entity Classes**: 
  - `Entity`: A simple base entity with a unique `Id` property.
  - `AuditableEntity`: Extends `Entity` with auditing fields like `CreatedAt`, `UpdatedAt`, `CreatedBy`, `UpdatedBy`, and `Version` for concurrency control.
- **User Context Interface**: `IUserContext` for managing user-related information.
- **Global Usings**: Automatically includes common namespaces like `System.ComponentModel.DataAnnotations` globally to reduce boilerplate code.

## Installation

Install the package via NuGet:

```bash
dotnet add package FluentCMS.Infrastructure.Common
```

## Usage

### Basic Entity

To create a simple entity, inherit from the `Entity` class:

```csharp
using FluentCMS.Infrastructure;

public class MyEntity : Entity
{
    public string Name { get; set; }
    // Add more properties as needed
}
```

### Auditable Entity

For entities that require auditing and versioning, inherit from `AuditableEntity`:

```csharp
using FluentCMS.Infrastructure;

public class MyAuditableEntity : AuditableEntity
{
    public string Title { get; set; }
    public string Content { get; set; }
    // Add more properties as needed
}
```

### User Context

To use the user context, you can implement or inject the `IUserContext` interface to access user information:

```csharp
using FluentCMS.Infrastructure;

public interface IUserContext
{
    // Define properties like UserId, Username, etc.
}

// Implement or inject IUserContext in your services.
```

## Dependencies

This library has no external dependencies. It targets .NET 10.0 and uses standard .NET frameworks.

## License

This project is licensed under the MIT License.