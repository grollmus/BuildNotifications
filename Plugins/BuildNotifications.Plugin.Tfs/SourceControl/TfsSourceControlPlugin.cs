using System;
using System.Threading.Tasks;
using Anotar.NLog;
using BuildNotifications.Plugin.Tfs.Configuration;
using BuildNotifications.PluginInterfaces;
using BuildNotifications.PluginInterfaces.Configuration;
using BuildNotifications.PluginInterfaces.SourceControl;

namespace BuildNotifications.Plugin.Tfs.SourceControl
{
    public class TfsSourceControlPlugin : TfsPlugin, ISourceControlPlugin
    {
        Task<ConnectionTestResult> ISourceControlPlugin.TestConnection(string serialized)
        {
            return TestConnection(serialized);
        }

        public override IPluginConfiguration Configuration => new TfsConfiguration(Host!.UiDispatcher);

        IBranchProvider? ISourceControlPlugin.ConstructProvider(string serialized)
        {
            var config = ParseConfig(serialized);
            if (config == null)
            {
                LogTo.Error("Given data was no TfsConfiguration");
                return null;
            }

            var connection = ConnectionPool.CreateConnection(config);
            if (connection == null)
                return null;

            if (config.Repository == null)
            {
                LogTo.Error("RepositoryId not given in connection data");
                return null;
            }

            if (config.Project == null)
            {
                LogTo.Error("ProjectId not given in connection data");
                return null;
            }

            return new TfsSourceControlProvider(connection, Guid.Parse(config.Repository.Id), Guid.Parse(config.Project.Id));
        }
    }
}