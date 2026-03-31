namespace FluentCMS.PluginSystem.Tests.Unit;

/// <summary>
/// Tests for <see cref="PluginInitializer.Initialize"/>.
/// PluginInitializer is internal; we access it via the internal assembly types through
/// the project reference which exposes it at compile time inside the same solution.
/// </summary>
public sealed class PluginInitializerTests
{
    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    private static PluginInitializer CreateInitializer(bool ignoreErrors = false)
    {
        var options = new PluginSystemOptions
        {
            IgnoreErrors = ignoreErrors,
            LoggerFactory = NullLoggerFactory.Instance
        };
        return new PluginInitializer(
            NullLogger<PluginInitializer>.Instance,
            options);
    }

    // -------------------------------------------------------------------------
    // Happy path
    // -------------------------------------------------------------------------

    [Fact]
    public void Initialize_ValidPluginType_ReturnsInitializedMetadata()
    {
        var initializer = CreateInitializer();
        var metadata = initializer.Initialize(typeof(ValidTestPlugin));

        metadata.Should().NotBeNull();
        metadata.Status.Should().Be(PluginStatus.Initialized);
        metadata.Type.Should().Be(typeof(ValidTestPlugin));
        metadata.Instance.Should().NotBeNull().And.BeAssignableTo<IPluginStartup>();
        metadata.Name.Should().Be("ValidTestPlugin");
        metadata.Version.Should().Be("1.0.0");
        metadata.ErrorMessage.Should().BeNull();
    }

    // -------------------------------------------------------------------------
    // Null / wrong-type guard
    // -------------------------------------------------------------------------

    [Fact]
    public void Initialize_NullType_ThrowsNullArgumentException()
    {
        var initializer = CreateInitializer();
        var act = () => initializer.Initialize(null!);
        act.Should().Throw<Exception>(); // NullArgumentException or ArgumentNullException
    }

    [Fact]
    public void Initialize_TypeThatDoesNotImplementIPluginStartup_WithIgnoreErrors_False_ThrowsPluginInitializerException()
    {
        var initializer = CreateInitializer(ignoreErrors: false);
        // string does not implement IPluginStartup
        var act = () => initializer.Initialize(typeof(string));
        act.Should().Throw<PluginInitializerException>();
    }

    [Fact]
    public void Initialize_TypeThatDoesNotImplementIPluginStartup_WithIgnoreErrors_True_ReturnsFailed()
    {
        var initializer = CreateInitializer(ignoreErrors: true);
        var metadata = initializer.Initialize(typeof(string));

        metadata.Status.Should().Be(PluginStatus.InitializeFailed);
        metadata.ErrorMessage.Should().NotBeNullOrEmpty();
    }

    // -------------------------------------------------------------------------
    // CancellationToken
    // -------------------------------------------------------------------------

    [Fact]
    public void Initialize_PreCancelledToken_ThrowsOperationCanceledException()
    {
        var initializer = CreateInitializer();
        var cts = new CancellationTokenSource();
        cts.Cancel();

        var act = () => initializer.Initialize(typeof(ValidTestPlugin), cts.Token);
        act.Should().Throw<OperationCanceledException>();
    }

    // -------------------------------------------------------------------------
    // IgnoreErrors = true on activation failure
    // -------------------------------------------------------------------------

    [Fact]
    public void Initialize_AbstractType_WithIgnoreErrors_True_ReturnsFailed()
    {
        var initializer = CreateInitializer(ignoreErrors: true);

        // An abstract type cannot be instantiated — Activator.CreateInstance will throw.
        var metadata = initializer.Initialize(typeof(AbstractPluginStub));

        metadata.Status.Should().Be(PluginStatus.InitializeFailed);
        metadata.ErrorMessage.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void Initialize_AbstractType_WithIgnoreErrors_False_ThrowsPluginInitializerException()
    {
        var initializer = CreateInitializer(ignoreErrors: false);

        var act = () => initializer.Initialize(typeof(AbstractPluginStub));
        act.Should().Throw<PluginInitializerException>();
    }

    // -------------------------------------------------------------------------
    // Stub helpers
    // -------------------------------------------------------------------------

    /// <summary>
    /// Abstract IPluginStartup implementor — cannot be instantiated.
    /// </summary>
    private abstract class AbstractPluginStub : IPluginStartup
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration? configuration) { }
        public void Configure(IApplicationBuilder app) { }
    }
}
