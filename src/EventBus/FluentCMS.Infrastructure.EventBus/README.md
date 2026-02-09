# FluentCMS.Infrastructure.EventBus

Event-driven communication abstractions for FluentCMS Infrastructure.

## Description

FluentCMS Infrastructure EventBus provides a set of abstractions and utilities for implementing event-driven communication between components in the FluentCMS ecosystem. It includes base classes and interfaces for domain events, their publication, and subscription, facilitating a decoupled architecture.

## Key Features

- `IEvent`: Marker interface for domain events, including properties for occurrence time and unique event ID.
- `EventBase`: Abstract base class for domain events that provides default implementations for common properties.
- `IEventSubscriber<TEvent>`: Interface for creating event handlers that respond to specific event types.
- `IEventPublisher`: Interface for publishing domain events to all registered subscribers.
- Dependency Injection extensions: `ServiceCollectionExtensions.AddEventHandler<TEvent, THandler>()` for registering event handlers in the service collection.

## Installation

Install via NuGet:

```bash
dotnet add package FluentCMS.Infrastructure.EventBus --version 1.0.0
```

*Replace `1.0.0` with the desired version.*

## Basic Usage

### Defining an Event

Create a class that inherits from `EventBase`:

```csharp
using FluentCMS.Infrastructure.EventBus;

public class UserCreatedEvent : EventBase
{
    public Guid UserId { get; init; }
    public string UserName { get; init; }
}
```

### Creating an Event Handler

Implement the `IEventSubscriber<TEvent>` interface:

```csharp
using FluentCMS.Infrastructure.EventBus.Abstractions;

public class UserCreatedHandler : IEventSubscriber<UserCreatedEvent>
{
    public Task Handle(UserCreatedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        // Handle the event, e.g., send email
        Console.WriteLine($"User {domainEvent.UserName} created.");
        return Task.CompletedTask;
    }
}
```

### Registering the Handler

Register the event handler using the extension method in your dependency injection setup (e.g., `Program.cs` or `Startup.cs`):

```csharp
using FluentCMS.Infrastructure.EventBus;

services.AddEventHandler<UserCreatedEvent, UserCreatedHandler>();
```

### Publishing an Event

Inject and use the `IEventPublisher` to publish events (note: an implementation of `IEventPublisher` must be provided separately):

```csharp
using FluentCMS.Infrastructure.EventBus.Abstractions;

// Assume _eventPublisher is injected (e.g., via constructor)
await _eventPublisher.Publish(new UserCreatedEvent
{
    UserId = Guid.NewGuid(),
    UserName = "JohnDoe"
});
```

## Dependencies

- `Microsoft.Extensions.DependencyInjection.Abstractions` (>= 10.0.2)

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.