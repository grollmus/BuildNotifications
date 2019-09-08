using ReflectSettings.Attributes;

namespace BuildNotifications.Plugin.DummyBuildServer
{
    public class Configuration
    {
        [MinMax(0, 99999)]
        public int Port { get; set; } = 1111;
    }
}