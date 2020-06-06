using System;
using System.Threading.Tasks;
using BuildNotifications.Plugin.Tfs.Configuration;
using BuildNotifications.PluginInterfaces;
using BuildNotifications.PluginInterfaces.Configuration;
using BuildNotifications.PluginInterfaces.SourceControl;
using JetBrains.Annotations;
using NLog.Fluent;

namespace BuildNotifications.Plugin.Tfs.SourceControl
{
    [UsedImplicitly]
    public class TfsSourceControlPlugin : TfsPlugin, ISourceControlPlugin
    {
        Task<ConnectionTestResult> ISourceControlPlugin.TestConnection(string serialized) => TestConnection(serialized);

        public override IPluginConfiguration ConstructNewConfiguration() => new TfsConfiguration(Host!.UiDispatcher);

        IBranchProvider? ISourceControlPlugin.ConstructProvider(string serialized)
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

            if (config.Repository == null)
            {
                Log.Error().Message("RepositoryId not given in connection data").Write();
                return null;
            }

            if (config.Project == null)
            {
                Log.Error().Message("ProjectId not given in connection data").Write();
                return null;
            }

            return new TfsSourceControlProvider(connection, Guid.Parse(config.Repository.Id), Guid.Parse(config.Project.Id));
        }
    }
}