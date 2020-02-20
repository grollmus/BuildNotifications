using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Anotar.NLog;
using BuildNotifications.Plugin.Tfs.Configuration;
using BuildNotifications.PluginInterfaces;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfaces.Configuration;
using BuildNotifications.PluginInterfaces.SourceControl;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;

namespace BuildNotifications.Plugin.Tfs
{
    public class TfsPlugin : ISourceControlPlugin, IBuildPlugin
    {
        public TfsPlugin()
        {
            TypeDescriptor.AddAttributes(typeof(IdentityDescriptor), new TypeConverterAttribute(typeof(IdentityDescriptorConverter).FullName));
            TypeDescriptor.AddAttributes(typeof(SubjectDescriptor), new TypeConverterAttribute(typeof(SubjectDescriptorConverter).FullName));

            Configuration = new TfsConfiguration();
            _connectionPool = new TfsConnectionPool();
        }

        private TfsConfigurationRawData? ParseConfig(string serialized)
        {
            var config = new TfsConfiguration();
            if (!config.Deserialize(serialized))
                return null;

            return config.AsRawData();
        }

        private async Task<ConnectionTestResult> TestConnection(string serialized)
        {
            var config = ParseConfig(serialized);

            if (config == null)
                return ConnectionTestResult.Failure(string.Empty);

            return await _connectionPool.TestConnection(config);
        }

        IBuildProvider? IBuildPlugin.ConstructProvider(string serialized)
        {
            var config = ParseConfig(serialized);
            if (config == null)
            {
                LogTo.Error("Given data was no TfsConfiguration");
                return null;
            }

            var connection = _connectionPool.CreateConnection(config);
            if (connection == null)
                return null;

            if (config.Project == null)
            {
                LogTo.Error("ProjectId not given in connection data");
                return null;
            }

            return new TfsBuildProvider(connection, Guid.Parse(config.Project.Id));
        }

        Task<ConnectionTestResult> IBuildPlugin.TestConnection(string serialized)
        {
            return TestConnection(serialized);
        }

        Task<ConnectionTestResult> ISourceControlPlugin.TestConnection(string serialized)
        {
            return TestConnection(serialized);
        }

        IBranchProvider? ISourceControlPlugin.ConstructProvider(string serialized)
        {
            var config = ParseConfig(serialized);
            if (config == null)
            {
                LogTo.Error("Given data was no TfsConfiguration");
                return null;
            }

            var connection = _connectionPool.CreateConnection(config);
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

        public IPluginConfiguration Configuration { get; }

        public string DisplayName => "Azure DevOps Server";

        public string IconSvgPath => "F1 M64,64z M0,0z M48.2,14.3L48.15,14.3 28.45,0 28.65,6.7 6.3,15.05 0,23.1 0,41.55 7.6,43.9 7.6,21.85 48.2,14.3 M64,11.2L48.25,14.3 48.25,41.7 48.25,49.05 7.65,43.9 24.35,63.9 24.15,55.7 48.05,64 64,50.5 64,11.2z";

        private readonly TfsConnectionPool _connectionPool;
    }
}