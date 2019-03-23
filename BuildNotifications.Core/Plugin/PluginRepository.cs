using System.Collections.Generic;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfaces.SourceControl;

namespace BuildNotifications.Core.Plugin
{
    internal class PluginRepository : IPluginRepository
    {
        /// <inheritdoc />
        public PluginRepository(IReadOnlyList<IBuildPlugin> build, IReadOnlyList<ISourceControlPlugin> sourceControl)
        {
            Build = build;
            SourceControl = sourceControl;
        }

        /// <inheritdoc />
        public IReadOnlyList<IBuildPlugin> Build { get; }

        /// <inheritdoc />
        public IReadOnlyList<ISourceControlPlugin> SourceControl { get; }
    }
}