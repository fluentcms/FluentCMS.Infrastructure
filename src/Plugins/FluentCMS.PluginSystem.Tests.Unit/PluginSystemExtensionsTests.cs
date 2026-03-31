namespace FluentCMS.PluginSystem.Tests.Unit;

/// <summary>
/// Tests for <see cref="PluginSystemExtensions.AddPluginSystem"/> and
/// <see cref="PluginSystemExtensions.UsePluginSystem"/>.
/// </summary>
public sealed class PluginSystemExtensionsTests
{
    // -------------------------------------------------------------------------
    // AddPluginSystem guard tests
    // -------------------------------------------------------------------------

    [Fact]
    public void AddPluginSystem_NullServices_ThrowsArgumentNullException()
    {
        IServiceCollection? services = null;
        var act = () => services!.AddPluginSystem(
            new ConfigurationBuilder().Build(),
            opts => opts.LoggerFactory = NullLoggerFactory.Instance);
        act.Should().Throw<ArgumentNullException>();
    }

    // -------------------------------------------------------------------------
    // LoggerFactory null safety
    // -------------------------------------------------------------------------

    [Fact]
    public void AddPluginSystem_NullLoggerFactory_ThrowsInvalidOperationException()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();

        var act = () => services.AddPluginSystem(configuration, opts =>
        {
            // Deliberately do NOT set LoggerFactory (leaves it as NullLoggerFactory.Instance
            // which is valid). Override with null to trigger the guard.
            opts.LoggerFactory = null!;
            opts.ScanAssemblyPatterns = [];   // no scanning to keep the test fast
        });

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*LoggerFactory*");
    }

    // -------------------------------------------------------------------------
    // UsePluginSystem guard tests
    // -------------------------------------------------------------------------

    [Fact]
    public void UsePluginSystem_NullAppBuilder_ThrowsArgumentNullException()
    {
        IApplicationBuilder? app = null;
        var act = () => app!.UsePluginSystem();
        act.Should().Throw<ArgumentNullException>();
    }

    // -------------------------------------------------------------------------
    // AddPluginSystem registers IPluginManager in DI
    // -------------------------------------------------------------------------

    [Fact]
    public void AddPluginSystem_ValidOptions_RegistersIPluginManagerSingleton()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();

        services.AddPluginSystem(configuration, opts =>
        {
            opts.LoggerFactory = NullLoggerFactory.Instance;
            opts.ScanAssemblyPatterns = [];  // empty patterns → nothing is scanned
        });

        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IPluginManager));
        descriptor.Should().NotBeNull("IPluginManager must be registered after AddPluginSystem");
        descriptor!.Lifetime.Should().Be(ServiceLifetime.Singleton);
    }

    // -------------------------------------------------------------------------
    // UsePluginSystem delegates to IPluginManager.Start
    // -------------------------------------------------------------------------

    [Fact]
    public void UsePluginSystem_CallsPluginManagerStart()
    {
        var managerMock = new Mock<IPluginManager>();
        managerMock.Setup(m => m.Start(It.IsAny<IApplicationBuilder>(), It.IsAny<CancellationToken>()));

        var services = new ServiceCollection();
        services.AddSingleton(managerMock.Object);
        var sp = services.BuildServiceProvider();

        var appMock = new Mock<IApplicationBuilder>();
        appMock.Setup(a => a.ApplicationServices).Returns(sp);

        appMock.Object.UsePluginSystem();

        managerMock.Verify(m => m.Start(appMock.Object, It.IsAny<CancellationToken>()), Times.Once);
    }
}
