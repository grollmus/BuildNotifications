using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Anotar.NLog;
using BuildNotifications.PluginInterfaces;
using BuildNotifications.PluginInterfaces.Builds;
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

            _connectionPool = new TfsConnectionPool();
        }

        private async Task<ConnectionTestResult> TestConnection(object data)
        {
            if (!(data is TfsConfiguration tfsConfiguration))
                return ConnectionTestResult.Failure(string.Empty);

            return await _connectionPool.TestConnection(tfsConfiguration);
        }

        IBuildProvider? IBuildPlugin.ConstructProvider(object? data)
        {
            if (!(data is TfsConfiguration configuration))
            {
                LogTo.Error("Given data was no TfsConfiguration");
                return null;
            }

            var connection = _connectionPool.CreateConnection(configuration);
            if (connection == null)
                return null;

            if (string.IsNullOrEmpty(configuration.ProjectId))
            {
                LogTo.Error("ProjectId not given in connection data");
                return null;
            }

            return new TfsBuildProvider(connection, configuration.ProjectId);
        }

        Task<ConnectionTestResult> IBuildPlugin.TestConnection(object data)
        {
            return TestConnection(data);
        }

        Task<ConnectionTestResult> ISourceControlPlugin.TestConnection(object data)
        {
            return TestConnection(data);
        }

        IBranchProvider? ISourceControlPlugin.ConstructProvider(object? data)
        {
            if (!(data is TfsConfiguration configuration))
            {
                LogTo.Error("Given data was no TfsConfiguration");
                return null;
            }

            var connection = _connectionPool.CreateConnection(configuration);
            if (connection == null)
                return null;

            if (string.IsNullOrEmpty(configuration.RepositoryId))
            {
                LogTo.Error("RepositoryId not given in connection data");
                return null;
            }

            return new TfsSourceControlProvider(connection, configuration.RepositoryId);
        }

        public string DisplayName => "Azure DevOps Server";

        public string IconSvgPath => "F1 M64,64z M0,0z M48.2,14.3L48.15,14.3 28.45,0 28.65,6.7 6.3,15.05 0,23.1 0,41.55 7.6,43.9 7.6,21.85 48.2,14.3 M64,11.2L48.25,14.3 48.25,41.7 48.25,49.05 7.65,43.9 24.35,63.9 24.15,55.7 48.05,64 64,50.5 64,11.2z";

        public Type GetConfigurationType()
        {
            return typeof(TfsConfiguration);
        }

        public void SetCurrentConfiguration(object instance)
        {
            throw new NotImplementedException();
        }

        public void ConfigurationChanged()
        {
            throw new NotImplementedException();
        }

        private TfsConnectionPool _connectionPool;
    }
}