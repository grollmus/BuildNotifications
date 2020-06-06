using System;
using System.ComponentModel;
using System.Threading.Tasks;
using BuildNotifications.PluginInterfaces;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfaces.SourceControl;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using NLog.Fluent;

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
                Log.Error().Message("Given data was no TfsConfiguration").Write();
                return null;
            }

            var connection = _connectionPool.CreateConnection(configuration);
            if (connection == null)
                return null;

            if (configuration.Project == null)
            {
                Log.Error().Message("ProjectId not given in connection data").Write();
                return null;
            }

            return new TfsBuildProvider(connection, Guid.Parse(configuration.Project.Id));
        }

        Task<ConnectionTestResult> IBuildPlugin.TestConnection(object data) => TestConnection(data);

        Task<ConnectionTestResult> ISourceControlPlugin.TestConnection(object data) => TestConnection(data);

        IBranchProvider? ISourceControlPlugin.ConstructProvider(object? data)
        {
            if (!(data is TfsConfiguration configuration))
            {
                Log.Error().Message("Given data was no TfsConfiguration").Write();
                return null;
            }

            var connection = _connectionPool.CreateConnection(configuration);
            if (connection == null)
                return null;

            if (configuration.Repository == null)
            {
                Log.Error().Message("RepositoryId not given in connection data").Write();
                return null;
            }

            if (configuration.Project == null)
            {
                Log.Error().Message("ProjectId not given in connection data").Write();
                return null;
            }

            return new TfsSourceControlProvider(connection, Guid.Parse(configuration.Repository.Id), Guid.Parse(configuration.Project.Id));
        }

        public string DisplayName => "Azure DevOps Server";

        public string IconSvgPath => "F1 M64,64z M0,0z M48.2,14.3L48.15,14.3 28.45,0 28.65,6.7 6.3,15.05 0,23.1 0,41.55 7.6,43.9 7.6,21.85 48.2,14.3 M64,11.2L48.25,14.3 48.25,41.7 48.25,49.05 7.65,43.9 24.35,63.9 24.15,55.7 48.05,64 64,50.5 64,11.2z";

        public Type GetConfigurationType() => typeof(TfsConfiguration);

        public void SetCurrentConfiguration(object instance)
        {
            throw new NotImplementedException();
        }

        public void ConfigurationChanged()
        {
            throw new NotImplementedException();
        }

        private readonly TfsConnectionPool _connectionPool;
    }
}