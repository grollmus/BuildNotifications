using System;
using System.Collections.Generic;
using System.ComponentModel;
using Anotar.NLog;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfaces.Host;
using BuildNotifications.PluginInterfaces.Options;
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

        IBuildProvider? IBuildPlugin.ConstructProvider(IReadOnlyDictionary<string, string?> data)
        {
            var connection = _connectionPool.CreateConnection(data);
            if (connection == null)
                return null;

            if (!data.TryGetValue(TfsConstants.Connection.ProjectId, out var projectId) || string.IsNullOrEmpty(projectId))
            {
                LogTo.Error("ProjectId not given in connection data");
                return null;
            }

            return new TfsBuildProvider(connection, projectId);
        }

        IOptionSchema IBuildPlugin.GetSchema(IPluginHost host)
        {
            var schema = host.SchemaFactory.Schema();

            schema.Add(host.SchemaFactory.Text(TfsConstants.Connection.Url, "Url", "Url of the TeamFoundation Server you want to connect to"));
            schema.Add(host.SchemaFactory.Text(TfsConstants.Connection.Collection, "Collection", "Name of the collection you want to connect to"));
            schema.Add(host.SchemaFactory.Text(TfsConstants.Connection.ProjectId, "Project", "Name of the project"));

            return schema;
        }

        IReadOnlyDictionary<string, string?> IBuildPlugin.Serialize(IBuildProvider provider)
        {
            throw new NotImplementedException();
        }

        IBranchProvider? ISourceControlPlugin.ConstructProvider(IReadOnlyDictionary<string, string?> data)
        {
            var connection = _connectionPool.CreateConnection(data);
            if (connection == null)
                return null;

            if (!data.TryGetValue(TfsConstants.Connection.RepositoryId, out var repositoryId) || string.IsNullOrEmpty(repositoryId))
            {
                LogTo.Error("RepositoryId not given in connection data");
                return null;
            }

            return new TfsSourceControlProvider(connection, repositoryId);
        }

        IOptionSchema ISourceControlPlugin.GetSchema(IPluginHost host)
        {
            throw new NotImplementedException();
        }

        IReadOnlyDictionary<string, string?> ISourceControlPlugin.Serialize(IBuildProvider provider)
        {
            throw new NotImplementedException();
        }

        private TfsConnectionPool _connectionPool;

        public string DisplayName => "Azure DevOps Server";

        public string IconSvgPath => "F1 M64,64z M0,0z M48.2,14.3L48.15,14.3 28.45,0 28.65,6.7 6.3,15.05 0,23.1 0,41.55 7.6,43.9 7.6,21.85 48.2,14.3 M64,11.2L48.25,14.3 48.25,41.7 48.25,49.05 7.65,43.9 24.35,63.9 24.15,55.7 48.05,64 64,50.5 64,11.2z";
    }
}