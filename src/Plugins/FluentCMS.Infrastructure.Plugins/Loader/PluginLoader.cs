namespace FluentCMS.Infrastructure.Plugins.Loader;

internal interface IPluginLoader
{
    List<Type> LoadPluginTypes(IEnumerable<string> assemblyFiles, CancellationToken cancellationToken = default);
}

internal class PluginLoader(ILogger<PluginLoader> logger, PluginSystemOptions pluginSystemOptions) : IPluginLoader
{
    private readonly ILogger<PluginLoader> _logger = NullArgumentException.RequireNonNull(logger);
    private readonly PluginSystemOptions _pluginSystemOptions = NullArgumentException.RequireNonNull(pluginSystemOptions);

    public List<Type> LoadPluginTypes(IEnumerable<string> assemblyFiles, CancellationToken cancellationToken = default)
    {
        NullArgumentException.ThrowIfNullOrEmpty(assemblyFiles);
        cancellationToken.ThrowIfCancellationRequested();
        var types = new List<Type>();

        foreach (var assemblyFile in assemblyFiles)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                var preloadedAssembly = FindLoaded(assemblyFile, cancellationToken);

                if (preloadedAssembly != null)
                {
                    _logger.LogDebug("Using preloaded assembly {Assembly}", assemblyFile);
                    types.AddRange(FindPluginTypes(preloadedAssembly, cancellationToken));
                }
                else
                {
                    // Not loaded: load into a collectible context
                    cancellationToken.ThrowIfCancellationRequested();
                    _logger.LogDebug("Loading assembly {Assembly} into collectible context", assemblyFile);
                    var alc = new PluginLoadContext(assemblyFile);

                    // Assembly loading can be cancelled in .NET 6+ if the method supports it
                    var newlyLoadedAssembly = alc.LoadFromAssemblyPath(assemblyFile);

                    cancellationToken.ThrowIfCancellationRequested();
                    var pluginTypes = FindPluginTypes(newlyLoadedAssembly, cancellationToken).ToArray();
                    if (pluginTypes.Length > 0)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        types.AddRange(pluginTypes);
                        _pluginSystemOptions.RegisteredALCs.Add(alc);
                    }
                    else
                    {
                        // No plugins -> unload ALC to prevent memory leaks from collectible ALCs
                        // Don't force synchronous GC - let the runtime handle cleanup naturally
                        _logger.LogDebug("No plugin types found in {Assembly}, unloading ALC", assemblyFile);
                        alc.Unload();
                        continue; // move to next assembly
                    }
                }

            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Plugin loading was cancelled while processing assembly {Assembly}", assemblyFile);
                throw;
            }
            catch (Exception ex)
            {
                if (_pluginSystemOptions.IgnoreErrors)
                {
                    _logger.LogError(ex, "Failed to load plugin from {AssemblyFile}, but continuing due to IgnoreErrors setting", assemblyFile);
                }
                else
                {
                    _logger.LogError(ex, "Failed to load plugin from {AssemblyFile}, stopping due to IgnoreErrors setting being false", assemblyFile);
                    throw new PluginLoaderException($"Failed to load plugin from {assemblyFile}", ex);
                }
            }
        }
        return types;
    }

    /// <summary>
    /// Attempts to find an already loaded assembly matching the given path.
    /// </summary>
    /// <param name="assemblyPath">The full path to the assembly file.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The loaded assembly if found; otherwise, null.</returns>
    private Assembly? FindLoaded(string assemblyPath, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            // Check if already loaded
            var asmName = AssemblyName.GetAssemblyName(assemblyPath);
            var loadedAsm = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => AssemblyName.ReferenceMatchesDefinition(a.GetName(), asmName));

            return loadedAsm;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error finding loaded assembly {Assembly}", assemblyPath);
            if (_pluginSystemOptions.IgnoreErrors)
            {
                _logger.LogWarning(ex, "Ignoring error finding loaded assembly {Assembly} due to configuration", assemblyPath);
                return null;
            }
            throw new PluginLoaderException($"Failed to find loaded assembly {assemblyPath}", ex);
        }
    }

    /// <summary>
    /// Finds plugin types in the given assembly that are marked with the PluginAttribute and implement IPluginStartup.
    /// </summary>
    /// <param name="assembly">The assembly to scan for plugin types.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>An enumerable of plugin types found in the assembly.</returns>
    private static IEnumerable<Type> FindPluginTypes(Assembly assembly, CancellationToken cancellationToken = default)
    {
        if (assembly.GetReferencedAssemblies().Any(a => a.Name == typeof(PluginAttribute).Assembly.GetName().Name))
        {
            foreach (var type in assembly.GetTypes())
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (type.IsDefined(typeof(PluginAttribute), inherit: false) &&
                    typeof(IPluginStartup).IsAssignableFrom(type) &&
                    type.IsClass && !type.IsAbstract &&
                    type.GetConstructor(Type.EmptyTypes) != null)
                {
                    yield return type;
                }
            }
        }
    }

}
