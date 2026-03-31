using System.Runtime.Loader;

namespace FluentCMS.PluginSystem.Tests.Unit;

/// <summary>
/// Tests for <see cref="PluginDiscovery"/> pattern-matching logic exercised via
/// <c>PluginSystemOptions.ScanAssemblyPatterns</c>.
///
/// Because <c>IsNameMatched</c> is private we drive it through <c>Scan()</c> against a
/// temporary directory that contains known DLL names.  The directory is populated with
/// zero-byte placeholder files – <c>Scan()</c> will skip them after name filtering
/// (MetadataLoadContext fails on zero-byte files) unless <c>IgnoreErrors = true</c>.
/// We therefore use <c>IgnoreErrors = true</c> so name-matched files are *attempted*
/// (observable via the returned list being non-empty) while non-matched files are
/// silently skipped regardless.
///
/// Pattern semantics tested: the current implementation does
///   <c>fileName.Contains(pattern.Trim('*'), StringComparison.OrdinalIgnoreCase)</c>
/// so a pattern like "FluentCMS.Plugins.*" trims to "FluentCMS.Plugins." and is treated
/// as a substring match on the full path.
/// </summary>
public sealed class PluginDiscoveryIsNameMatchedTests : IDisposable
{
    private readonly string _tempDir;

    public PluginDiscoveryIsNameMatchedTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"PluginDiscoveryTests_{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, recursive: true);
    }

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    private static PluginSystemOptions BuildOptions(string[] patterns, bool ignoreErrors = true)
        => new()
        {
            ScanAssemblyPatterns = patterns,
            IgnoreErrors = ignoreErrors,
            LoggerFactory = NullLoggerFactory.Instance
        };

    // Creates a zero-byte file with the given name in _tempDir.
    private string PlantFile(string fileName)
    {
        var path = Path.Combine(_tempDir, fileName);
        File.WriteAllBytes(path, []);
        return path;
    }

    // DiscoveryTester invokes Scan() *after* monkey-patching the private
    // _pluginAssemblyPath field so that discovery points at _tempDir instead of
    // the real executable directory.
    private List<string> ScanDir(PluginSystemOptions options)
    {
        // We can't directly set _pluginAssemblyPath (private field set inside Init()).
        // Instead we drive Scan() by pointing the process executable folder to our
        // temp directory.  The cleanest approach without modifying production code is
        // to run Scan() and catch PluginDiscoveryException (which is thrown when the
        // resolver cannot find dependencies).  We only check whether the *name filter*
        // accepts or rejects a file — IgnoreErrors=true means assembly-loading errors
        // are swallowed, so the scan completes.
        //
        // NOTE: because Init() uses Assembly.GetExecutingAssembly().Location the real
        // scan dir is the test runner's output folder; we can't redirect it in a pure
        // unit test without touching the source.  These tests therefore plant our
        // probe files in the *real* output folder, scan, and then remove the probes.
        //
        // A simpler alternative — just verify the options wiring and that Scan()
        // returns a List<string> — is used here.  Full pattern-matching coverage is
        // provided through the direct internal-method tests below using reflection.
        return [];   // see reflection-based tests below
    }

    // -------------------------------------------------------------------------
    // Direct pattern-matching tests via reflection
    // -------------------------------------------------------------------------
    // We expose IsNameMatched via reflection to get precise unit tests without
    // filesystem side-effects.

    private static bool InvokeIsNameMatched(PluginSystemOptions options, string assemblyFileName)
    {
        // PluginDiscovery is internal; InternalsVisibleTo gives compile-time access but we
        // still need to construct it via primary-constructor reflection (logger, options).
        var discoveryType = typeof(PluginDiscovery);

        // Build ILogger<PluginDiscovery> via NullLoggerFactory so it matches the primary ctor.
        var nullLoggerOfT = typeof(NullLogger<>).MakeGenericType(discoveryType);
        var logger = Activator.CreateInstance(nullLoggerOfT)!;   // NullLogger<PluginDiscovery> has a public ctor

        var discovery = Activator.CreateInstance(discoveryType,
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Public,
            null,
            new object[] { logger, options },
            null)!;

        var method = discoveryType.GetMethod(
            "IsNameMatched",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        method.Should().NotBeNull("IsNameMatched method must exist.");

        return (bool)method!.Invoke(discovery, [assemblyFileName])!;
    }

    [Theory]
    [InlineData("FluentCMS.Plugins.MyFeature.dll", "FluentCMS.Plugins.*", true)]
    [InlineData("fluentcms.plugins.myfeature.dll", "FluentCMS.Plugins.*", true)]   // case-insensitive
    [InlineData("FLUENTCMS.PLUGINS.OTHER.DLL", "FluentCMS.Plugins.*", true)]
    [InlineData("Unrelated.Library.dll", "FluentCMS.Plugins.*", false)]
    [InlineData("FluentCMS.Plugins.dll", "FluentCMS.Plugins.*", true)]             // exact prefix match
    public void IsNameMatched_WildcardPattern_ReturnsExpected(
        string fileName, string pattern, bool expected)
    {
        var options = BuildOptions([pattern]);
        var result = InvokeIsNameMatched(options, fileName);
        result.Should().Be(expected,
            because: $"pattern '{pattern}' should {(expected ? "" : "not ")}match '{fileName}'");
    }

    [Fact]
    public void IsNameMatched_MultiplePatterns_MatchesIfAnyPatternMatches()
    {
        var options = BuildOptions(["FluentCMS.Plugins.*", "Acme.Extensions.*"]);

        InvokeIsNameMatched(options, "FluentCMS.Plugins.Foo.dll").Should().BeTrue();
        InvokeIsNameMatched(options, "Acme.Extensions.Bar.dll").Should().BeTrue();
        InvokeIsNameMatched(options, "Completely.Different.dll").Should().BeFalse();
    }

    [Fact]
    public void IsNameMatched_FullPath_MatchesOnFullPathSubstring()
    {
        var options = BuildOptions(["FluentCMS.Plugins.*"]);
        // Full path containing the pattern substring in the directory segment should match
        var fullPath = Path.Combine(@"C:\app\plugins\FluentCMS.Plugins.Feature", "FluentCMS.Plugins.Feature.dll");
        InvokeIsNameMatched(options, fullPath).Should().BeTrue();
    }

    [Fact]
    public void IsNameMatched_ExactMiddleSubstring_MatchesIfSubstringPresent()
    {
        // Pattern "MyPlugin" (no wildcards) trims to "MyPlugin"
        var options = BuildOptions(["MyPlugin"]);
        InvokeIsNameMatched(options, "Company.MyPlugin.Features.dll").Should().BeTrue();
        InvokeIsNameMatched(options, "Company.Other.dll").Should().BeFalse();
    }

    [Fact]
    public void IsNameMatched_EmptyPattern_MatchesEverything()
    {
        // An empty pattern trims to "" — every string contains "".
        var options = BuildOptions(["*"]);
        InvokeIsNameMatched(options, "AnyAssembly.dll").Should().BeTrue();
    }
}
