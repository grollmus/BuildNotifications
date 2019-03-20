using BuildNotifications.Core.Plugin.Options;
using BuildNotifications.PluginInterfaces.Host;
using BuildNotifications.PluginInterfaces.Options;

namespace BuildNotifications.Core.Plugin.Host
{
    internal class PluginHost : IPluginHost
    {
        public IOptionSchemaFactory SchemaFactory => new OptionSchemaFactory();
    }
}