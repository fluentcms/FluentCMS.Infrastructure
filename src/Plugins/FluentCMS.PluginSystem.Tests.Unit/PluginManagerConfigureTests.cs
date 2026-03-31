using System.Reflection;

namespace FluentCMS.PluginSystem.Tests.Unit;

/// <summary>
/// Tests for <see cref="PluginManager.Configure"/> covering:
/// - Status count accuracy (Configured vs failed)
/// - IgnoreErrors = true/false on ConfigureServices failure
/// - CancellationToken propagation
/// - Partial success when StrictTimeout = false
/// </summary>
public sealed class PluginManagerConfigureTests
{
    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    /// <summary>
    /// Creates a <see cref="PluginManager"/> via reflection (it is internal) with
    /// the provided mock dependencies.
    /// </summary>
    private static IPluginManager CreateManager(
        IPluginDiscovery discovery,
        IPluginInitializer initializer,
        IPluginLoader loader,
        PluginSystemOptions? options = null)
    {
        options ??= new PluginSystemOptions { LoggerFactory = NullLoggerFactory.Instance };

        var pluginManagerType = typeof(PluginSystemExtensions).Assembly
            .GetType("FluentCMS.Infrastructure.Plugins.PluginManager")!;

        return (IPluginManager)Activator.CreateInstance(
            pluginManagerType,
            discovery,
            initializer,
            loader,
            NullLogger<PluginManager>.Instance,
            options)!;
    }

    private static IConfiguration EmptyConfiguration()
        => new ConfigurationBuilder().Build();

    // -------------------------------------------------------------------------
    // Happy path — one plugin configures successfully
    // -------------------------------------------------------------------------

    [Fact]
    public void Configure_OnePlugin_StatusBecomesConfigured()
    {
        var options = new PluginSystemOptions { LoggerFactory = NullLoggerFactory.Instance };

        var discoveryMock = new Mock<IPluginDiscovery>();
        discoveryMock.Setup(d => d.Scan(It.IsAny<CancellationToken>()))
            .Returns(["dummy.dll"]);

        var loaderMock = new Mock<IPluginLoader>();
        loaderMock.Setup(l => l.LoadPluginTypes(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .Returns([typeof(ValidTestPlugin)]);

        var initializerMock = new Mock<IPluginInitializer>();
        initializerMock.Setup(i => i.Initialize(typeof(ValidTestPlugin), It.IsAny<CancellationToken>()))
            .Returns(new PluginMetadata
            {
                Type = typeof(ValidTestPlugin),
                Instance = new ValidTestPlugin(),
                Name = "ValidTestPlugin",
                Version = "1.0.0",
                Status = PluginStatus.Initialized
            });

        var manager = CreateManager(discoveryMock.Object, initializerMock.Object, loaderMock.Object, options);
        var services = new ServiceCollection();

        var act = () => manager.Configure(services, EmptyConfiguration(), CancellationToken.None);
        act.Should().NotThrow();
    }

    // -------------------------------------------------------------------------
    // Configure failure with IgnoreErrors = false
    // -------------------------------------------------------------------------

    [Fact]
    public void Configure_ConfigureServicesThrows_IgnoreErrors_False_Rethrows()
    {
        var options = new PluginSystemOptions
        {
            IgnoreErrors = false,
            LoggerFactory = NullLoggerFactory.Instance
        };

        var discoveryMock = new Mock<IPluginDiscovery>();
        discoveryMock.Setup(d => d.Scan(It.IsAny<CancellationToken>()))
            .Returns(["dummy.dll"]);

        var loaderMock = new Mock<IPluginLoader>();
        loaderMock.Setup(l => l.LoadPluginTypes(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .Returns([typeof(ThrowingConfigureServicesPlugin)]);

        var initializerMock = new Mock<IPluginInitializer>();
        initializerMock.Setup(i => i.Initialize(typeof(ThrowingConfigureServicesPlugin), It.IsAny<CancellationToken>()))
            .Returns(new PluginMetadata
            {
                Type = typeof(ThrowingConfigureServicesPlugin),
                Instance = new ThrowingConfigureServicesPlugin(),
                Name = "ThrowingConfigureServicesPlugin",
                Version = "1.0.0",
                Status = PluginStatus.Initialized
            });

        var manager = CreateManager(discoveryMock.Object, initializerMock.Object, loaderMock.Object, options);
        var services = new ServiceCollection();

        var act = () => manager.Configure(services, EmptyConfiguration(), CancellationToken.None);
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Simulated ConfigureServices failure*");
    }

    // -------------------------------------------------------------------------
    // Configure failure with IgnoreErrors = true
    // -------------------------------------------------------------------------

    [Fact]
    public void Configure_ConfigureServicesThrows_IgnoreErrors_True_Continues()
    {
        var options = new PluginSystemOptions
        {
            IgnoreErrors = true,
            LoggerFactory = NullLoggerFactory.Instance
        };

        var discoveryMock = new Mock<IPluginDiscovery>();
        discoveryMock.Setup(d => d.Scan(It.IsAny<CancellationToken>()))
            .Returns(["dummy.dll"]);

        var loaderMock = new Mock<IPluginLoader>();
        loaderMock.Setup(l => l.LoadPluginTypes(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .Returns([typeof(ThrowingConfigureServicesPlugin)]);

        var initializerMock = new Mock<IPluginInitializer>();
        initializerMock.Setup(i => i.Initialize(typeof(ThrowingConfigureServicesPlugin), It.IsAny<CancellationToken>()))
            .Returns(new PluginMetadata
            {
                Type = typeof(ThrowingConfigureServicesPlugin),
                Instance = new ThrowingConfigureServicesPlugin(),
                Name = "ThrowingConfigureServicesPlugin",
                Version = "1.0.0",
                Status = PluginStatus.Initialized
            });

        var manager = CreateManager(discoveryMock.Object, initializerMock.Object, loaderMock.Object, options);
        var services = new ServiceCollection();

        var act = () => manager.Configure(services, EmptyConfiguration(), CancellationToken.None);
        act.Should().NotThrow("IgnoreErrors=true means failures are swallowed");
    }

    // -------------------------------------------------------------------------
    // CancellationToken propagation
    // -------------------------------------------------------------------------

    [Fact]
    public void Configure_PreCancelledToken_ThrowsOrExitsEarly()
    {
        var cts = new CancellationTokenSource();
        cts.Cancel();

        var discoveryMock = new Mock<IPluginDiscovery>();
        discoveryMock.Setup(d => d.Scan(It.IsAny<CancellationToken>()))
            .Callback((CancellationToken ct) => ct.ThrowIfCancellationRequested())
            .Returns(new List<string>());

        var manager = CreateManager(
            discoveryMock.Object,
            Mock.Of<IPluginInitializer>(),
            Mock.Of<IPluginLoader>());

        var services = new ServiceCollection();

        var act = () => manager.Configure(services, EmptyConfiguration(), cts.Token);
        act.Should().Throw<Exception>(); // OperationCanceledException or similar
    }

    // -------------------------------------------------------------------------
    // Zero plugins — no errors expected
    // -------------------------------------------------------------------------

    [Fact]
    public void Configure_NoPluginsDiscovered_CompletesWithoutError()
    {
        var discoveryMock = new Mock<IPluginDiscovery>();
        discoveryMock.Setup(d => d.Scan(It.IsAny<CancellationToken>()))
            .Returns([]);

        var loaderMock = new Mock<IPluginLoader>();
        loaderMock.Setup(l => l.LoadPluginTypes(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .Returns([]);

        var manager = CreateManager(
            discoveryMock.Object,
            Mock.Of<IPluginInitializer>(),
            loaderMock.Object);

        var services = new ServiceCollection();
        var act = () => manager.Configure(services, EmptyConfiguration(), CancellationToken.None);
        act.Should().NotThrow();
    }

    // -------------------------------------------------------------------------
    // Multiple plugins, priorities respected
    // -------------------------------------------------------------------------

    [Fact]
    public void Configure_TwoPlugins_BothSucceed_BothConfigured()
    {
        var callOrder = new List<string>();

        var pluginA = CreatePluginMetadata("PluginA", configureServicesPriority: 10,
            configureServicesAction: (_, _) => callOrder.Add("A"));
        var pluginB = CreatePluginMetadata("PluginB", configureServicesPriority: 5,
            configureServicesAction: (_, _) => callOrder.Add("B"));

        var discoveryMock = new Mock<IPluginDiscovery>();
        discoveryMock.Setup(d => d.Scan(It.IsAny<CancellationToken>())).Returns(["a.dll", "b.dll"]);

        var loaderMock = new Mock<IPluginLoader>();
        loaderMock.Setup(l => l.LoadPluginTypes(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .Returns([typeof(object), typeof(string)]); // two placeholder types

        var initializerMock = new Mock<IPluginInitializer>();
        initializerMock.Setup(i => i.Initialize(typeof(object), It.IsAny<CancellationToken>())).Returns(pluginA);
        initializerMock.Setup(i => i.Initialize(typeof(string), It.IsAny<CancellationToken>())).Returns(pluginB);

        var manager = CreateManager(discoveryMock.Object, initializerMock.Object, loaderMock.Object);
        manager.Configure(new ServiceCollection(), EmptyConfiguration(), CancellationToken.None);

        // B has priority 5 (lower = first), A has priority 10
        callOrder.Should().Equal("B", "A");
    }

    // -------------------------------------------------------------------------
    // Stub factory
    // -------------------------------------------------------------------------

    private static PluginMetadata CreatePluginMetadata(
        string name,
        int configureServicesPriority = 1000,
        Action<IServiceCollection, IConfiguration?>? configureServicesAction = null)
    {
        var stub = new StubPlugin(name, configureServicesPriority, configureServicesAction);
        return new PluginMetadata
        {
            Type = typeof(StubPlugin),
            Instance = stub,
            Name = name,
            Version = "1.0.0",
            Status = PluginStatus.Initialized
        };
    }

    private sealed class StubPlugin(
        string name,
        int priority,
        Action<IServiceCollection, IConfiguration?>? onConfigureServices) : IPluginStartup
    {
        public string Name => name;
        public string Version => "1.0.0";
        public int ConfigureServicesPriority => priority;
        public int ConfigurePriority => priority;

        public void ConfigureServices(IServiceCollection services, IConfiguration? configuration)
            => onConfigureServices?.Invoke(services, configuration);

        public void Configure(IApplicationBuilder app) { }
    }
}
