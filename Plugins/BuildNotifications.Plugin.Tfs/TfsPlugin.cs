using System.ComponentModel;
using System.Threading.Tasks;
using BuildNotifications.Plugin.Tfs.Configuration;
using BuildNotifications.PluginInterfaces;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfaces.Configuration;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;

namespace BuildNotifications.Plugin.Tfs
{
    public abstract class TfsPlugin : IPlugin
    {
        protected TfsPlugin()
        {
            TypeDescriptor.AddAttributes(typeof(IdentityDescriptor), new TypeConverterAttribute(typeof(IdentityDescriptorConverter).FullName));
            TypeDescriptor.AddAttributes(typeof(SubjectDescriptor), new TypeConverterAttribute(typeof(SubjectDescriptorConverter).FullName));

            ConnectionPool = new TfsConnectionPool();
        }

        private protected TfsConfigurationRawData? ParseConfig(string serialized)
        {
            var config = new TfsConfiguration();
            if (!config.Deserialize(serialized))
                return null;

            return config.AsRawData();
        }

        protected async Task<ConnectionTestResult> TestConnection(string serialized)
        {
            var config = ParseConfig(serialized);

            if (config == null)
                return ConnectionTestResult.Failure(string.Empty);

            return await ConnectionPool.TestConnection(config);
        }

        public abstract IPluginConfiguration Configuration { get; }
        public string DisplayName => "Azure DevOps Server";

        public string IconSvgPath => "F1 M64,64z M0,0z M48.2,14.3L48.15,14.3 28.45,0 28.65,6.7 6.3,15.05 0,23.1 0,41.55 7.6,43.9 7.6,21.85 48.2,14.3 M64,11.2L48.25,14.3 48.25,41.7 48.25,49.05 7.65,43.9 24.35,63.9 24.15,55.7 48.05,64 64,50.5 64,11.2z";

        private protected readonly TfsConnectionPool ConnectionPool;
    }
}