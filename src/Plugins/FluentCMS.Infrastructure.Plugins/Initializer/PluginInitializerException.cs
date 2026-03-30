using System.Runtime.Serialization;

namespace FluentCMS.Infrastructure.Plugins.Initializer;

/// <summary>
/// Exception thrown when plugin initialization fails.
/// </summary>
[Serializable]
public class PluginInitializerException : Exception
{
    /// <summary>
    /// Initializes a new instance of the PluginInitializerException class.
    /// </summary>
    public PluginInitializerException()
    {
    }

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

    /// <summary>
    /// Initializes a new instance of the PluginInitializerException class with serialized data (CA2229).
    /// </summary>
    /// <param name="info">The SerializationInfo that holds the serialized object data.</param>
    /// <param name="context">The StreamingContext that contains contextual information about the source or destination.</param>
    protected PluginInitializerException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
