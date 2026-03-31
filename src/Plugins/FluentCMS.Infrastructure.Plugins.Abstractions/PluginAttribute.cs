namespace FluentCMS.Infrastructure.Plugins.Abstractions;

/// <summary>
/// Attribute that marks a class as a plugin startup class.
/// Classes marked with this attribute will be automatically discovered and loaded by the plugin system.
/// </summary>
/// <remarks>
/// <para>
/// <b>Design decision — discovery marker only:</b>
/// <c>PluginAttribute</c> is intentionally a pure discovery marker with no metadata properties.
/// Its sole responsibility is to signal to the plugin discovery mechanism that the decorated class
/// should be treated as a plugin entry point.
/// </para>
/// <para>
/// All plugin metadata — including <b>load priority</b>, version, name, and any future runtime
/// configuration — belongs exclusively on <see cref="IPluginStartup"/>. This keeps a single,
/// unambiguous ownership boundary:
/// <list type="bullet">
///   <item><description><c>PluginAttribute</c> = static discovery signal (applied before the assembly is loaded into a live context).</description></item>
///   <item><description><c>IPluginStartup</c> = runtime metadata and lifecycle methods (<c>ConfigureServices</c>, <c>Configure</c>, priorities, version, …).</description></item>
/// </list>
/// </para>
/// <para>
/// Do <b>not</b> add priority, dependency, or versioning properties to this attribute.
/// Placing metadata in two locations would create conflicting values with no specified tiebreaker
/// and would force developers to choose between two inconsistent sources of truth.
/// </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class PluginAttribute : Attribute
{
}
