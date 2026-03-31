using FluentCMS.Infrastructure.Plugins.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FluentCMS.PluginSystem.TestPlugins;

/// <summary>
/// A test plugin whose <see cref="IPluginStartup.Configure"/> always throws.
/// Used to verify IgnoreErrors and error-propagation logic in PluginManager.Start().
/// </summary>
[Plugin]
public sealed class ThrowingConfigurePlugin : IPluginStartup
{
    public string Name => "ThrowingConfigurePlugin";
    public string Version => "1.0.0";
    public int ConfigureServicesPriority => 500;
    public int ConfigurePriority => 500;

    public void ConfigureServices(IServiceCollection services, IConfiguration? configuration) { }

    public void Configure(IApplicationBuilder app)
        => throw new InvalidOperationException("Simulated Configure failure.");
}
