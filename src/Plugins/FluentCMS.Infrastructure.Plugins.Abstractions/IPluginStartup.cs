using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FluentCMS.Infrastructure.Plugins.Abstractions;

/// <summary>
/// Central interface for all plugins in the FluentCMS plugin system.
/// Plugins must implement this interface and be marked with <see cref="PluginAttribute"/>.
/// </summary>
/// <remarks>
/// <para>
/// <b>Design decision — single source of truth for plugin metadata:</b>
/// <c>IPluginStartup</c> is the <b>sole owner</b> of all plugin metadata and runtime configuration,
/// including name, version, and load priorities. The companion <see cref="PluginAttribute"/> is
/// intentionally a pure discovery marker with no metadata properties.
/// </para>
/// <para>
/// This single-ownership rule prevents conflicting values (e.g., priority 1 on the attribute vs.
/// priority 5 on the interface) and gives plugin authors one clear location for all configuration.
/// </para>
/// </remarks>
public interface IPluginStartup
{
    public virtual string Name => GetType().Assembly.GetName().Name ?? string.Empty;

    /// <summary>
    /// Gets the version of the plugin. Defaults to the assembly version.
    /// </summary>
    public virtual string Version => GetType().Assembly.GetName().Version?.ToString() ?? "1.0.0";

    /// <summary>
    /// Gets the priority for service registration. Lower values are processed first.
    /// Default value is 1000.
    /// </summary>
    public virtual int ConfigureServicesPriority => 1000;

    /// <summary>
    /// Gets the priority for middleware configuration. Lower values are processed first.
    /// Default value is 1000.
    /// </summary>
    public virtual int ConfigurePriority => 1000;

    /// <summary>
    /// Configures the services for this plugin. This is called during the service registration phase.
    /// The configuration parameter is pre-scoped to this plugin's configuration section.
    /// </summary>
    /// <param name="services">The service collection to register services with.</param>
    /// <param name="configuration">Plugin-specific configuration, scoped to "Plugins:{PluginName}".</param>
    void ConfigureServices(IServiceCollection services, IConfiguration? configuration);

    /// <summary>
    /// Configures the application pipeline for this plugin. This is called during the middleware configuration phase.
    /// Use this to register middleware, endpoints, or other application features.
    /// </summary>
    /// <param name="app">The application builder to configure.</param>
    void Configure(IApplicationBuilder app);
}
