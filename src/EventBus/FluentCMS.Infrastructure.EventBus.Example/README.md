# FluentCMS.Infrastructure.EventBus.Example

## Description
This project provides an example of using the EventBus for FluentCMS Infrastructure. It demonstrates event publishing and handling with sample implementations for UserRegisteredEvent and OrderPlacedEvent.

## Key Features
- Sample event handlers (SendOrderConfirmation, UpdateInventory for OrderPlacedEvent; CreateUserProfile, LogUserActivity, SendWelcomeEmail for UserRegisteredEvent)
- Event bus modes: Aggregate and FailFast
- Dependency injection setup with logging
- Performance measurement

## Installation
```bash
dotnet add package FluentCMS.Infrastructure.EventBus.Example
```

## Usage
```csharp
var serviceProvider = new ServiceCollection()
    .AddEventBusInMemory()
    .AddLogging()
    .AddEventHandlersFromAssembly(typeof(Program).Assembly)
    .BuildServiceProvider();

var eventBus = serviceProvider.GetRequiredService<IEventBus>();

// Publish event
await eventBus.PublishAsync(new OrderPlacedEvent { OrderId = "123", CustomerId = "456", TotalAmount = 99.99m, ItemCount = 1 });
```

## Dependencies
- EventBus abstractions: FluentCMS.Infrastructure.EventBus, FluentCMS.Infrastructure.EventBus.InMemory
- Microsoft.Extensions.DependencyInjection (10.0.2)
- Microsoft.Extensions.Logging.Console (10.0.2)

## License
MIT
