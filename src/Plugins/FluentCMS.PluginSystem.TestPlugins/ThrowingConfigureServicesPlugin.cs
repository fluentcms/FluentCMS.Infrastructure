using FluentCMS.Infrastructure.Plugins.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FluentCMS.PluginSystem.TestPlugins;

/// <summary>
/// A test plugin whose <see cref="IPluginStartup.ConfigureServices"/> always throws.
/// Used to verify IgnoreErrors and error-propagation logic in PluginManager.Configure().
/// </summary>
[Plugin]
public sealed class ThrowingConfigureServicesPlugin : IPluginStartup
{
    public string Name => "ThrowingConfigureServicesPlugin";
    public string Version => "1.0.0";
    public int ConfigureServicesPriority => 500;
    public int ConfigurePriority => 500;

    public void ConfigureServices(IServiceCollection services, IConfiguration? configuration)
        => throw new InvalidOperationException("Simulated ConfigureServices failure.");

    public void Configure(IApplicationBuilder app) { }
}
