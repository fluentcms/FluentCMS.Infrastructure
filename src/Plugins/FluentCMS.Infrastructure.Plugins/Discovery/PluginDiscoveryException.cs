using System.Runtime.Serialization;

namespace FluentCMS.Infrastructure.Plugins.Discovery;

/// <summary>
/// Exception thrown when plugin discovery fails.
/// </summary>
[Serializable]
public class PluginDiscoveryException : Exception
{
    /// <summary>
    /// Initializes a new instance of the PluginDiscoveryException class.
    /// </summary>
    public PluginDiscoveryException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the PluginDiscoveryException class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public PluginDiscoveryException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the PluginDiscoveryException class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public PluginDiscoveryException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the PluginDiscoveryException class with serialized data (CA2229).
    /// </summary>
    /// <param name="info">The SerializationInfo that holds the serialized object data.</param>
    /// <param name="context">The StreamingContext that contains contextual information about the source or destination.</param>
    protected PluginDiscoveryException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
