using System.Runtime.Serialization;

namespace FluentCMS.Infrastructure.Plugins.Loader;

/// <summary>
/// Exception thrown when plugin loading fails.
/// </summary>
[Serializable]
public class PluginLoaderException : Exception
{
    /// <summary>
    /// Initializes a new instance of the PluginLoaderException class.
    /// </summary>
    public PluginLoaderException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the PluginLoaderException class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public PluginLoaderException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the PluginLoaderException class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public PluginLoaderException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the PluginLoaderException class with serialized data (CA2229).
    /// </summary>
    /// <param name="info">The SerializationInfo that holds the serialized object data.</param>
    /// <param name="context">The StreamingContext that contains contextual information about the source or destination.</param>
    protected PluginLoaderException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
