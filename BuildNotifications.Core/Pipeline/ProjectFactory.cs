using System;
using Anotar.NLog;
using BuildNotifications.Core.Config;
using BuildNotifications.Core.Plugin;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfaces.SourceControl;

namespace BuildNotifications.Core.Pipeline
{
    internal class ProjectFactory : IProjectFactory
    {
        public ProjectFactory(IPluginRepository pluginRepository)
        {
            _pluginRepository = pluginRepository;
        }

        private IBranchProvider? BranchProvider(IConnectionData connectionData)
        {
            var sourceControlPlugin = _pluginRepository.FindSourceControlPlugin(connectionData.PluginType);
            if (sourceControlPlugin == null)
            {
                return null;
            }

            IBranchProvider? branchProvider;
            try
            {
                branchProvider = sourceControlPlugin.ConstructProvider(connectionData.Options);
            }
            catch (Exception e)
            {
                LogTo.ErrorException($"Failed to construct branch provider from plugin {sourceControlPlugin.GetType()}", e);
                return null;
            }

            return branchProvider;
        }

        private IBuildProvider? BuildProvider(IConnectionData buildConnectionData)
        {
            var buildPlugin = _pluginRepository.FindBuildPlugin(buildConnectionData.PluginType);
            if (buildPlugin == null)
            {
                return null;
            }

            IBuildProvider? buildProvider;
            try
            {
                buildProvider = buildPlugin.ConstructProvider(buildConnectionData.Options);
            }
            catch (Exception e)
            {
                LogTo.ErrorException($"Failed to construct build provider from plugin {buildPlugin.GetType()}", e);
                return null;
            }
            
            return buildProvider;
        }

        /// <inheritdoc />
        public IProject? Construct(IConnectionData buildConnectionData, IConnectionData sourceConnectionData)
        {
            LogTo.Debug($"Trying to construct project from {buildConnectionData.Name} and {sourceConnectionData.Name}");

            var buildProvider = BuildProvider(buildConnectionData);
            if (buildProvider == null)
            {
                LogTo.Error("Failed to construct build provider");
                return null;
            }

            var branchProvider = BranchProvider(sourceConnectionData);
            if (branchProvider == null)
            {
                LogTo.Error("Failed to construct branch provider");
                return null;
            }

            return new Project(buildProvider, branchProvider);
        }

        private readonly IPluginRepository _pluginRepository;
    }
}