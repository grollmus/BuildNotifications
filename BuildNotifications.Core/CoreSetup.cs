using System;
using System.Threading.Tasks;
using BuildNotifications.Core.Config;
using BuildNotifications.Core.Pipeline;
using BuildNotifications.Core.Pipeline.Tree;
using BuildNotifications.Core.Plugin;
using BuildNotifications.Core.Utilities;

namespace BuildNotifications.Core
{
    public class CoreSetup
    {
        public CoreSetup(string configFilePath)
        {
            _configFilePath = configFilePath;
            var serializer = new Serializer();

            var pluginLoader = new PluginLoader();
            PluginRepository = pluginLoader.LoadPlugins(new[] {"plugins"});

            _configurationSerializer = new ConfigurationSerializer(serializer, PluginRepository);
            Configuration = LoadConfiguration(configFilePath);

            ProjectProvider = new ProjectProvider(Configuration, PluginRepository);

            var branchNameExtractor = new BranchNameExtractor();
            var treeBuilder = new TreeBuilder(Configuration, branchNameExtractor);
            Pipeline = new Pipeline.Pipeline(treeBuilder, Configuration);

            Pipeline.Notifier.Updated += Notifier_Updated;
            Pipeline.Notifier.ErrorOccured += Notifier_ErrorOccured;
        }

        public IConfiguration Configuration { get; }

        public IPipeline Pipeline { get; }

        public IPluginRepository PluginRepository { get; }

        public IProjectProvider ProjectProvider { get; }

        public event EventHandler<PipelineUpdateEventArgs> PipelineUpdated;

        public event EventHandler<PipelineErrorEventArgs> ErrorOccurred;

        public void PersistConfigurationChanges()
        {
            _configurationSerializer.Save(Configuration, _configFilePath);
        }

        public Task Update()
        {
            return Pipeline.Update();
        }

        private IConfiguration LoadConfiguration(string configFilePath)
        {
            return _configurationSerializer.Load(configFilePath);
        }

        private void Notifier_Updated(object? sender, PipelineUpdateEventArgs e)
        {
            PipelineUpdated?.Invoke(this, e);
        }

        private void Notifier_ErrorOccured(object sender, PipelineErrorEventArgs e)
        {
            ErrorOccurred?.Invoke(this, e);
        }

        private readonly string _configFilePath;
        private readonly ConfigurationSerializer _configurationSerializer;
    }
}