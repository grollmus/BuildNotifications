namespace BuildNotifications.Core
{
    public interface IPathResolver
    {
        string ConfigurationFolder { get; }
        string PredefinedConfigurationFileName { get; }
        string PredefinedConfigurationFilePath { get; }
        string UserConfigurationFileName { get; }
        string UserConfigurationFilePath { get; }
    }
}