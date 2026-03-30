namespace FluentCMS.Infrastructure.Plugins;

public static class PluginSystemExtensions
{
    public static IServiceCollection AddPluginSystem(this IServiceCollection services, IConfiguration configuration, Action<PluginSystemOptions>? configureOptions = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(services);

        var options = new PluginSystemOptions();
        configureOptions?.Invoke(options);

        var pluginManager = new PluginManager(
            new PluginDiscovery(options.CreateLogger<PluginDiscovery>(), options),
            new PluginInitializer(options.CreateLogger<PluginInitializer>(), options),
            new PluginLoader(options.CreateLogger<PluginLoader>(), options),
            options.CreateLogger<PluginManager>(),
            options
        );

        pluginManager.Configure(services, configuration, cancellationToken);

        services.AddSingleton<IPluginManager>(pluginManager);

        return services;
    }

    private static ILogger<T> CreateLogger<T>(this PluginSystemOptions options)
    {
        if (options.LoggerFactory is null)
        {
            throw new InvalidOperationException(
                $"{nameof(PluginSystemOptions)}.{nameof(PluginSystemOptions.LoggerFactory)} must not be null. " +
                "Set it in the options delegate passed to AddPluginSystem(), e.g.: options.LoggerFactory = loggerFactory;");
        }
        return options.LoggerFactory.CreateLogger<T>();
    }

    public static IApplicationBuilder UsePluginSystem(this IApplicationBuilder app, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(app);

        var pluginManager = app.ApplicationServices.GetRequiredService<IPluginManager>();
        pluginManager.Start(app, cancellationToken);

        return app;
    }
}
