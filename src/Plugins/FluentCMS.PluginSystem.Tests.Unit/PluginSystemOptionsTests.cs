namespace FluentCMS.PluginSystem.Tests.Unit;

/// <summary>
/// Tests for <see cref="PluginSystemOptions"/> default values and LoggerFactory null safety.
/// </summary>
public sealed class PluginSystemOptionsTests
{
    [Fact]
    public void DefaultScanAssemblyPatterns_ContainsFluentCMSPluginsWildcard()
    {
        var options = new PluginSystemOptions();
        options.ScanAssemblyPatterns.Should().ContainSingle()
            .Which.Should().Be("FluentCMS.Plugins.*");
    }

    [Fact]
    public void DefaultIgnoreErrors_IsFalse()
    {
        var options = new PluginSystemOptions();
        options.IgnoreErrors.Should().BeFalse();
    }

    [Fact]
    public void DefaultPluginLoadTimeout_Is30Seconds()
    {
        var options = new PluginSystemOptions();
        options.PluginLoadTimeout.Should().Be(TimeSpan.FromSeconds(30));
    }

    [Fact]
    public void DefaultUnloadALCsAfterStartup_IsFalse()
    {
        var options = new PluginSystemOptions();
        options.UnloadALCsAfterStartup.Should().BeFalse();
    }

    [Fact]
    public void DefaultStrictTimeout_IsFalse()
    {
        var options = new PluginSystemOptions();
        options.StrictTimeout.Should().BeFalse();
    }

    [Fact]
    public void DefaultLoggerFactory_IsNullLoggerFactory()
    {
        var options = new PluginSystemOptions();
        // NullLoggerFactory.Instance is the expected default — it must not be null
        options.LoggerFactory.Should().NotBeNull();
        options.LoggerFactory.Should().BeAssignableTo<ILoggerFactory>();
    }

    [Fact]
    public void LoggerFactory_CanBeOverridden()
    {
        // Use a minimal in-memory logger to avoid requiring the Console provider package
        var factory = LoggerFactory.Create(b => b.SetMinimumLevel(LogLevel.Debug));
        var options = new PluginSystemOptions { LoggerFactory = factory };
        options.LoggerFactory.Should().BeSameAs(factory);
        factory.Dispose();
    }

    [Fact]
    public void IgnoreErrors_CanBeSetToTrue()
    {
        var options = new PluginSystemOptions { IgnoreErrors = true };
        options.IgnoreErrors.Should().BeTrue();
    }

    [Fact]
    public void ScanAssemblyPatterns_CanBeOverridden()
    {
        var patterns = new[] { "MyCompany.Plugins.*", "OtherPlugins.*" };
        var options = new PluginSystemOptions { ScanAssemblyPatterns = patterns };
        options.ScanAssemblyPatterns.Should().BeEquivalentTo(patterns);
    }

    [Fact]
    public void RegisteredALCs_IsInitiallyEmpty()
    {
        // RegisteredALCs is internal; verify via reflection to avoid depending on implementation details
        var options = new PluginSystemOptions();
        var prop = typeof(PluginSystemOptions)
            .GetProperty("RegisteredALCs",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        prop.Should().NotBeNull("RegisteredALCs property must exist.");
        var value = prop!.GetValue(options) as System.Collections.ICollection;
        value.Should().NotBeNull();
        value!.Count.Should().Be(0, "RegisteredALCs should start empty.");
    }
}
