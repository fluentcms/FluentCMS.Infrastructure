namespace FluentCMS.Infrastructure.Plugins;

internal interface IPluginManager
{
    void Configure(IServiceCollection services, IConfiguration rootConfiguration, CancellationToken cancellationToken);
    void Start(IApplicationBuilder app, CancellationToken cancellationToken);
}

internal class PluginManager(IPluginDiscovery pluginDiscovery, IPluginInitializer pluginInitializer, IPluginLoader pluginLoader, ILogger<PluginManager> logger, PluginSystemOptions pluginSystemOptions) : IPluginManager
{
    private readonly ILogger<PluginManager> _logger = NullArgumentException.RequireNonNull(logger);
    private readonly PluginSystemOptions _pluginSystemOptions = NullArgumentException.RequireNonNull(pluginSystemOptions);
    private readonly List<PluginMetadata> _pluginMetadataList = [];

    private void Init(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting plugin initialization process...");
        var assemblyFiles = pluginDiscovery.Scan(cancellationToken);
        var pluginTypes = pluginLoader.LoadPluginTypes(assemblyFiles, cancellationToken);

        foreach (var pluginType in pluginTypes)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var metadata = pluginInitializer.Initialize(pluginType, cancellationToken);
            _pluginMetadataList.Add(metadata);
        }
        _logger.LogInformation("PluginManager initialized with {Count} plugins", _pluginMetadataList.Count);
    }

    public void Configure(IServiceCollection services, IConfiguration rootConfiguration, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting plugin configuration process with timeout {Timeout}s", _pluginSystemOptions.PluginLoadTimeout.TotalSeconds);

        // Create a timeout-based cancellation token source
        using var timeoutCts = new CancellationTokenSource(_pluginSystemOptions.PluginLoadTimeout);
        using var combinedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

        try
        {
            Init(combinedCts.Token);

            // run configure services for each plugin in order
            foreach (var pluginMetadata in _pluginMetadataList.Where(p => p.Status == PluginStatus.Initialized).OrderBy(p => p.Instance!.ConfigureServicesPriority))
            {
                combinedCts.Token.ThrowIfCancellationRequested();
                try
                {
                    pluginMetadata.Instance!.ConfigureServices(services, rootConfiguration);
                    pluginMetadata.Status = PluginStatus.Configured;
                    _logger.LogInformation("Configured services for plugin {Plugin} version {Version}", pluginMetadata.Name, pluginMetadata.Version);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogWarning("Plugin configuration was cancelled during ConfigureServices for plugin {Plugin}", pluginMetadata.Name);
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during ConfigureServices of plugin {Plugin}, but continuing due to IgnoreErrors setting", pluginMetadata.Name);
                    if (_pluginSystemOptions.IgnoreErrors)
                    {
                        _logger.LogWarning("Ignoring ConfigureServices error for plugin {Plugin} due to configuration.", pluginMetadata.Name);
                        pluginMetadata.ErrorMessage = ex.Message;
                        pluginMetadata.Status = PluginStatus.ConfigurationFailed;
                        continue;
                    }
                    throw;
                }
            }
            _logger.LogInformation("Plugin configuration process completed. {Count} plugins loaded.", _pluginMetadataList.Count(p => p.Status == PluginStatus.Configured));
            var failedCount = _pluginMetadataList.Count(p => p.Status == PluginStatus.ConfigurationFailed || p.Status == PluginStatus.InitializeFailed || p.Status == PluginStatus.NotInitialized);
            if (failedCount > 0)
                _logger.LogWarning("Plugins configuration process with errors: {Count}", failedCount);
        }
        catch (OperationCanceledException)
        {
            var cancelledByTimeout = timeoutCts.IsCancellationRequested;
            var timeoutMessage = cancelledByTimeout ?
                $"Plugin loading timed out after {_pluginSystemOptions.PluginLoadTimeout.TotalSeconds} seconds" :
                "Plugin loading was cancelled";

            _logger.LogError("{Message}. Loaded {Loaded} plugins successfully before cancellation.", timeoutMessage, _pluginMetadataList.Count(p => p.Status == PluginStatus.Configured));

            if (_pluginSystemOptions.StrictTimeout || !cancelledByTimeout)
            {
                _logger.LogError("Strict timeout enabled or cancellation was not from timeout, throwing exception");
                throw;
            }
            else
            {
                _logger.LogWarning("Timeout occurred but StrictTimeout is disabled, continuing with partially loaded plugins");
                // Continue with whatever was loaded successfully - this leaves the system in a partially configured state
            }
        }
    }

    public void Start(IApplicationBuilder app, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting plugin startup process...");
        foreach (var pluginMetadata in _pluginMetadataList.Where(p => p.Status == PluginStatus.Configured).OrderBy(p => p.Instance!.ConfigurePriority))
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                pluginMetadata.Instance!.Configure(app);
                pluginMetadata.Status = PluginStatus.Started;
                _logger.LogInformation("Startup plugin {Plugin} version {Version}", pluginMetadata.Name, pluginMetadata.Version);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Plugin startup was cancelled during startup of plugin {Plugin}", pluginMetadata.Name);
                pluginMetadata.Status = PluginStatus.StartFailed;
                pluginMetadata.ErrorMessage = "Startup was cancelled";
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during startup of plugin {Plugin}, but continuing due to IgnoreErrors setting", pluginMetadata.Name);
                if (_pluginSystemOptions.IgnoreErrors)
                {
                    _logger.LogWarning("Ignoring startup error for plugin {Plugin} due to configuration.", pluginMetadata.Name);
                    pluginMetadata.ErrorMessage = ex.Message;
                    pluginMetadata.Status = PluginStatus.StartFailed;
                    continue;
                }

                throw;
            }
        }
        _logger.LogInformation("Plugin startup process completed. {Count} plugins started.", _pluginMetadataList.Count(p => p.Status == PluginStatus.Started));
        var failedCount = _pluginMetadataList.Count(p => p.Status != PluginStatus.Started);
        if (failedCount > 0)
            _logger.LogWarning("Plugins startup process with errors: {Count}", failedCount);

        // Unload ALCs if configured to reduce memory usage
        if (_pluginSystemOptions.UnloadALCsAfterStartup)
        {
            _logger.LogInformation("Unloading registered ALCs after startup to reduce memory footprint.");
            var unloadedCount = 0;
            foreach (var alc in _pluginSystemOptions.RegisteredALCs)
            {
                try
                {
                    alc.Unload();
                    unloadedCount++;
                    _logger.LogDebug("Unloaded ALC for reduction of memory footprint");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error unloading ALC during cleanup.");
                }
            }
            _pluginSystemOptions.RegisteredALCs.Clear();
            _logger.LogInformation("Successfully unloaded {Count} ALCs. Memory will be reclaimed during next garbage collection cycle.", unloadedCount);
        }
    }
}
