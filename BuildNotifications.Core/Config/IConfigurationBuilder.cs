namespace BuildNotifications.Core.Config
{
    public interface IConfigurationBuilder
    {
        IProjectConfiguration EmptyConfiguration(string name);
    }
}