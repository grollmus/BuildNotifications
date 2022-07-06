using BuildNotifications.Core.Text;

namespace BuildNotifications.Core.Config;

/// <summary>
/// Represents the data stored for a single connection.
/// </summary>
public class ConnectionData
{
    public ConnectionData()
    {
        Name = StringLocalizer.NewConnection;
    }

    /// <summary>
    /// Type of this connection.
    /// </summary>
    public ConnectionPluginType ConnectionType { get; set; }

    /// <summary>
    /// Display name of the connection.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Serialized RawConfiguration for the selected BuildPlugin to use
    /// </summary>
    public string? PluginConfiguration { get; set; }

    /// <summary>
    /// Type name of the plugin that is able to construct a build provider
    /// for this connection.
    /// </summary>
    public string? PluginType { get; set; }
}