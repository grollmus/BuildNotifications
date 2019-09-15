using System;
using System.Threading.Tasks;
using Anotar.NLog;
using BuildNotifications.Core.Config;
using BuildNotifications.Core.Pipeline;
using BuildNotifications.Core.Pipeline.Notification.Distribution;
using BuildNotifications.Core.Pipeline.Tree;
using BuildNotifications.Core.Plugin;
using BuildNotifications.Core.Utilities;

namespace BuildNotifications.Core
{
    public class CoreSetup
    {
        public CoreSetup(IPathResolver pathResolver, IDistributedNotificationReceiver? notificationReceiver)
        {
            _pathResolver = pathResolver;
            var serializer = new Serializer();

            var pluginLoader = new PluginLoader();
            PluginRepository = pluginLoader.LoadPlugins(pathResolver.PluginFolders);

            _configurationSerializer = new ConfigurationSerializer(serializer, PluginRepository);
            var configurationBuilder = new ConfigurationBuilder(_pathResolver, _configurationSerializer);
            Configuration = configurationBuilder.LoadConfiguration();

            ProjectProvider = new ProjectProvider(Configuration, PluginRepository);

            var branchNameExtractor = new BranchNameExtractor();
            var searcher = new BuildSearcher();
            var treeBuilder = new TreeBuilder(Configuration, branchNameExtractor, searcher);
            Pipeline = new Pipeline.Pipeline(treeBuilder, Configuration);

            Pipeline.Notifier.Updated += Notifier_Updated;

            if (notificationReceiver != null)
            {
                NotificationReceiver = notificationReceiver;
                NotificationReceiver.DistributedNotificationReceived += (sender, args) => DistributedNotificationReceived?.Invoke(this, args);
            }
        }

        public IConfiguration Configuration { get; }

        public IDistributedNotificationReceiver? NotificationReceiver { get; }

        public IPipeline Pipeline { get; }

        public IPluginRepository PluginRepository { get; }

        public IProjectProvider ProjectProvider { get; }

        public event EventHandler<DistributedNotificationReceivedEventArgs> DistributedNotificationReceived;

        public event EventHandler<PipelineUpdateEventArgs> PipelineUpdated;

        public void PersistConfigurationChanges()
        {
            var configFilePath = _pathResolver.UserConfigurationFilePath;
            LogTo.Info($"Persisting configuration to path {configFilePath}");
            _configurationSerializer.Save(Configuration, configFilePath);
        }

        public Task Update()
        {
            return Pipeline.Update();
        }

        private void Notifier_Updated(object? sender, PipelineUpdateEventArgs e)
        {
            PipelineUpdated?.Invoke(this, e);
        }

        private readonly IPathResolver _pathResolver;

        private readonly ConfigurationSerializer _configurationSerializer;
    }
}