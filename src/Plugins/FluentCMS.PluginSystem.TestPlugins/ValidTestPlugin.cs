using FluentCMS.Infrastructure.Plugins.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FluentCMS.PluginSystem.TestPlugins;

/// <summary>
/// A correctly formed test plugin used in unit and integration tests.
/// Has both [Plugin] attribute and implements IPluginStartup with a parameterless constructor.
/// </summary>
[Plugin]
public sealed class ValidTestPlugin : IPluginStartup
{
    public string Name => "ValidTestPlugin";
    public string Version => "1.0.0";
    public int ConfigureServicesPriority => 100;
    public int ConfigurePriority => 100;

    public void ConfigureServices(IServiceCollection services, IConfiguration? configuration)
    {
        services.AddSingleton<ValidTestPluginMarker>();
    }

    public void Configure(IApplicationBuilder app)
    {
        // No middleware needed for tests
    }
}

/// <summary>
/// A sentinel type registered by <see cref="ValidTestPlugin"/> so integration tests can verify the plugin was loaded.
/// </summary>
public sealed class ValidTestPluginMarker { }
