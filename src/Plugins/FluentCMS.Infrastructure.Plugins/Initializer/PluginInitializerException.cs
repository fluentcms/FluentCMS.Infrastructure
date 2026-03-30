namespace FluentCMS.Infrastructure.Plugins.Initializer;

/// <summary>
/// Exception thrown when plugin initialization fails.
/// </summary>
public class PluginInitializerException : Exception
{
    /// <summary>
    /// Initializes a new instance of the PluginInitializerException class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public PluginInitializerException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the PluginInitializerException class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public PluginInitializerException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
