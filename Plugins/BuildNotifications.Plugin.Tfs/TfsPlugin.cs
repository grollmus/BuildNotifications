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
            {
                return null;
            }

            if (!data.TryGetValue(TfsConstants.Connection.ProjectId, out var projectId) || string.IsNullOrEmpty(projectId))
            {
                LogTo.Error("ProjectId not given in connection data");
                return null;
            }

            return new TfsBuildProvider(connection, projectId);
        }

        IOptionSchema IBuildPlugin.GetSchema(IPluginHost host)
        {
            throw new NotImplementedException();
        }

        IReadOnlyDictionary<string, string?> IBuildPlugin.Serialize(IBuildProvider provider)
        {
            throw new NotImplementedException();
        }

        IBranchProvider? ISourceControlPlugin.ConstructProvider(IReadOnlyDictionary<string, string?> data)
        {
            var connection = _connectionPool.CreateConnection(data);
            if (connection == null)
            {
                return null;
            }

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
    }
}