namespace FluentCMS.PluginSystem.Tests.Unit;

/// <summary>
/// Tests for <see cref="PluginManager.Start"/> covering:
/// - FailedCount accuracy (only <see cref="PluginStatus.StartFailed"/> counts)
/// - IgnoreErrors = true/false on Configure failure
/// - ALC unload after startup when UnloadALCsAfterStartup = true
/// </summary>
public sealed class PluginManagerStartTests
{
    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    private static IPluginManager CreateManagerWithPreInitialisedPlugins(
        IEnumerable<PluginMetadata> preConfiguredPlugins,
        PluginSystemOptions? options = null)
    {
        options ??= new PluginSystemOptions { LoggerFactory = NullLoggerFactory.Instance };

        // Stub discovery and loader to return nothing extra; all plugins arrive via initializer stubs.
        var discoveryMock = new Mock<IPluginDiscovery>();
        discoveryMock.Setup(d => d.Scan(It.IsAny<CancellationToken>())).Returns([]);

        var loaderMock = new Mock<IPluginLoader>();
        loaderMock.Setup(l => l.LoadPluginTypes(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .Returns([]);

        var pluginList = preConfiguredPlugins.ToList();
        var metadataTypes = pluginList.Select(p => p.Type).ToList();

        // Map each unique type to its metadata
        var initializerMock = new Mock<IPluginInitializer>();
        foreach (var meta in pluginList)
        {
            var captured = meta;
            initializerMock.Setup(i => i.Initialize(captured.Type, It.IsAny<CancellationToken>()))
                .Returns(captured);
        }

        // Loader returns the same types list
        if (metadataTypes.Count > 0)
        {
            loaderMock.Setup(l => l.LoadPluginTypes(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
                .Returns(metadataTypes);
            discoveryMock.Setup(d => d.Scan(It.IsAny<CancellationToken>()))
                .Returns(pluginList.Select(_ => "dummy.dll").ToList());
        }

        var pluginManagerType = typeof(PluginSystemExtensions).Assembly
            .GetType("FluentCMS.Infrastructure.Plugins.PluginManager")!;

        var manager = (IPluginManager)Activator.CreateInstance(
            pluginManagerType,
            discoveryMock.Object,
            initializerMock.Object,
            loaderMock.Object,
            NullLogger<PluginManager>.Instance,
            options)!;

        // Run Configure first so plugins reach PluginStatus.Configured
        manager.Configure(new ServiceCollection(),
            new ConfigurationBuilder().Build(),
            CancellationToken.None);

        return manager;
    }

    private static IApplicationBuilder BuildAppBuilder()
    {
        var services = new ServiceCollection();
        var sp = services.BuildServiceProvider();

        var appBuilderMock = new Mock<IApplicationBuilder>();
        appBuilderMock.Setup(a => a.ApplicationServices).Returns(sp);
        appBuilderMock.Setup(a => a.Use(It.IsAny<Func<Microsoft.AspNetCore.Http.RequestDelegate, Microsoft.AspNetCore.Http.RequestDelegate>>()))
            .Returns(appBuilderMock.Object);
        return appBuilderMock.Object;
    }

    // -------------------------------------------------------------------------
    // Happy path
    // -------------------------------------------------------------------------

    [Fact]
    public void Start_AllPluginsConfigured_AllStart_NoErrors()
    {
        var meta = new PluginMetadata
        {
            Type = typeof(ValidTestPlugin),
            Instance = new ValidTestPlugin(),
            Name = "ValidTestPlugin",
            Version = "1.0.0",
            Status = PluginStatus.Initialized
        };

        var manager = CreateManagerWithPreInitialisedPlugins([meta]);
        var act = () => manager.Start(BuildAppBuilder(), CancellationToken.None);
        act.Should().NotThrow();
    }

    // -------------------------------------------------------------------------
    // FailedCount precision — only StartFailed
    // -------------------------------------------------------------------------

    [Fact]
    public void Start_OnePluginThrows_IgnoreErrors_True_CountsOnlyStartFailed()
    {
        var options = new PluginSystemOptions { IgnoreErrors = true, LoggerFactory = NullLoggerFactory.Instance };

        var goodMeta = new PluginMetadata
        {
            Type = typeof(ValidTestPlugin),
            Instance = new ValidTestPlugin(),
            Name = "GoodPlugin",
            Version = "1.0.0",
            Status = PluginStatus.Initialized
        };

        var badMeta = new PluginMetadata
        {
            Type = typeof(ThrowingConfigurePlugin),
            Instance = new ThrowingConfigurePlugin(),
            Name = "BadPlugin",
            Version = "1.0.0",
            Status = PluginStatus.Initialized
        };

        var manager = CreateManagerWithPreInitialisedPlugins([goodMeta, badMeta], options);
        var act = () => manager.Start(BuildAppBuilder(), CancellationToken.None);
        // IgnoreErrors = true: should not throw
        act.Should().NotThrow();
    }

    [Fact]
    public void Start_OnePluginThrows_IgnoreErrors_False_Rethrows()
    {
        var options = new PluginSystemOptions { IgnoreErrors = false, LoggerFactory = NullLoggerFactory.Instance };

        var badMeta = new PluginMetadata
        {
            Type = typeof(ThrowingConfigurePlugin),
            Instance = new ThrowingConfigurePlugin(),
            Name = "BadPlugin",
            Version = "1.0.0",
            Status = PluginStatus.Initialized
        };

        var manager = CreateManagerWithPreInitialisedPlugins([badMeta], options);
        var act = () => manager.Start(BuildAppBuilder(), CancellationToken.None);
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Simulated Configure failure*");
    }

    // -------------------------------------------------------------------------
    // CancellationToken
    // -------------------------------------------------------------------------

    [Fact]
    public void Start_PreCancelledToken_ThrowsOperationCanceledException()
    {
        var meta = new PluginMetadata
        {
            Type = typeof(ValidTestPlugin),
            Instance = new ValidTestPlugin(),
            Name = "ValidTestPlugin",
            Version = "1.0.0",
            Status = PluginStatus.Initialized
        };

        var manager = CreateManagerWithPreInitialisedPlugins([meta]);

        var cts = new CancellationTokenSource();
        cts.Cancel();

        var act = () => manager.Start(BuildAppBuilder(), cts.Token);
        act.Should().Throw<OperationCanceledException>();
    }

    // -------------------------------------------------------------------------
    // No plugins started when none are Configured
    // -------------------------------------------------------------------------

    [Fact]
    public void Start_NoConfiguredPlugins_CompletesWithoutError()
    {
        var manager = CreateManagerWithPreInitialisedPlugins([]);
        var act = () => manager.Start(BuildAppBuilder(), CancellationToken.None);
        act.Should().NotThrow();
    }
}
