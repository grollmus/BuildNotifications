using System;
using System.Threading.Tasks;
using BuildNotifications.Plugin.Tfs.Configuration;
using BuildNotifications.PluginInterfaces;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfaces.Configuration;
using NLog.Fluent;

namespace BuildNotifications.Plugin.Tfs.Build
{
    public class TfsBuildPlugin : TfsPlugin, IBuildPlugin
    {
        IBuildProvider? IBuildPlugin.ConstructProvider(string serialized)
        {
            var config = ParseConfig(serialized);
            if (config == null)
            {
                Log.Error().Message("Given data was no TfsConfiguration").Write();
                return null;
            }

            var connection = ConnectionPool.CreateConnection(config);
            if (connection == null)
                return null;

            if (config.Project == null)
            {
                Log.Error().Message("ProjectId not given in connection data").Write();
                return null;
            }

            return new TfsBuildProvider(connection, Guid.Parse(config.Project.Id));
        }

        Task<ConnectionTestResult> IBuildPlugin.TestConnection(string serialized) => TestConnection(serialized);
        public override IPluginConfiguration ConstructNewConfiguration() => new TfsConfiguration(Host!.UiDispatcher, ConfigurationFlags.HideRepository);
    }
}