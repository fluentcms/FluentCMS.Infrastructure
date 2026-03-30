# FluentCMS.Infrastructure.EventBus.Tests

> Test suite for the EventBus components in FluentCMS infrastructure, providing comprehensive unit tests to ensure reliable event publishing, handling, and service configurations.

## 📖 About
This package contains unit tests for the EventBus infrastructure in FluentCMS. It includes tests for event publishing, event handlers, service collection extensions, and various event bus implementations such as the in-memory provider. These tests are designed to validate the functionality and reliability of the EventBus system.

**Note:** This package is intended for testing and development purposes only. It is not meant for production use.

## 🚀 Installation

Install the package via NuGet:

```bash
dotnet add package FluentCMS.Infrastructure.EventBus.Tests
```

## 🛠 Key Features
- Unit tests for the `EventBase` class, ensuring proper event ID generation and property initialization.
- Tests for `ServiceCollectionExtensions`, verifying event handler registration and DI integration.
- Coverage for in-memory event publishing, including subscriber invocation, error handling modes, and aggregated exceptions.
- Comprehensive test coverage using xUnit, FluentAssertions, and Moq for mocking.

## 📚 Dependencies
This test suite relies on the following key dependencies:
- .NET 10.0
- xunit.v3 (3.2.2)
- FluentAssertions (8.8.0)
- Moq (4.20.72)
- Microsoft.NET.Test.Sdk (18.0.1)
- xunit.runner.visualstudio (3.1.5)
- coverlet.collector (6.0.4)
- Microsoft.Extensions.DependencyInjection (10.0.2)
- Microsoft.Extensions.Logging (10.0.2)

Note: The package also has project references to the main EventBus and InMemory components for testing.

## 📜 License
This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.