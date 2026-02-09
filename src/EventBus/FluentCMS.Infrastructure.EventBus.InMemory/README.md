# FluentCMS.Infrastructure.EventBus.InMemory

An in-memory event bus implementation for FluentCMS Infrastructure, designed for fast, in-process event handling.

## Features

- **Fast and efficient**: Provides low-latency event handling entirely within the process.
- **Multiple handlers support**: Allows multiple handlers per event type.
- **Error aggregation**: Aggregates exceptions from multiple handlers for better error management.
- **Configurable options**: Supports different error handling modes.

## Installation

Install the package from NuGet:

```
dotnet add package FluentCMS.Infrastructure.EventBus.InMemory
```

## Usage

1. Register the event bus in your dependency injection container:

```csharp
services.AddInMemoryEventBus();
```

2. Publish events using the `IEventPublisher` interface:

```csharp
public class SomeService
{
    private readonly IEventPublisher _eventPublisher;

    public SomeService(IEventPublisher eventPublisher)
    {
        _eventPublisher = eventPublisher;
    }

    public async Task PublishEvent()
    {
        var @event = new SampleEvent();
        await _eventPublisher.PublishAsync(@event);
    }
}
```

3. Implement event handlers by inheriting from `IEventHandler<TEvent>`:

```csharp
public class SampleEventHandler : IEventHandler<SampleEvent>
{
    public async Task HandleEventAsync(SampleEvent @event)
    {
        // Handle the event
    }
}
```

Handlers are automatically resolved and invoked by the event publisher.

## Dependencies

- **EventBus abstractions**: `FluentCMS.Infrastructure.EventBus`
- **Framework**:
  - `Microsoft.AspNetCore.Http` (2.3.9)
  - `Microsoft.Extensions.Logging.Abstractions` (10.0.2)
  - `Microsoft.Extensions.Options` (10.0.2)

## License

This project is licensed under the [MIT License](https://opensource.org/licenses/MIT).