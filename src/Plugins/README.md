# Plugins

> A modular plugin system for FluentCMS infrastructure, enabling dynamic discovery, loading, and initialization of plugins to extend core functionality.

## 📖 About
The `Plugins` project provides a flexible and robust plugin architecture for FluentCMS. It allows seamless discovery, loading, and management of plugins, enabling developers to extend the CMS capabilities dynamically. The system emphasizes modularity, safe loading of assemblies, and straightforward initialization, making it easy to build and integrate additional features.

## 🚀 Getting Started

### Prerequisites
* .NET 10.0 Runtime and SDK

### Installation
Since this project is a library designed to be integrated within a larger application, include the relevant DLLs or package references into your main application. Ensure the plugin assemblies are placed in accessible directories for discovery.

### Usage Example
```csharp
// Example of integrating the plugin system during application startup
var pluginDiscovery = new PluginDiscovery();
var plugins = pluginDiscovery.Scan("path/to/plugin/directory");
var pluginLoader = new PluginLoader();
var loadedPlugins = plugins.Select(plugin => pluginLoader.Load(plugin));
foreach (var plugin in loadedPlugins)
{
    // Initialize plugin
}
```

## 🔧 Features
- **Plugin Discovery**: Automatically scans directories for plugin assemblies.
- **Safe Loading**: Uses a custom AssemblyLoadContext for isolated and collectible plugin loading.
- **Dynamic Initialization**: Instantiates and initializes plugins implementing `IPluginStartup`.
- **Robust Error Handling**: Custom exceptions and logging support error diagnostics.
- **Extensible Architecture**: Based on interfaces, attributes, and abstraction for easy extension and customization.