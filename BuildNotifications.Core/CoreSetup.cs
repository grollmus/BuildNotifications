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
        private readonly string _configFilePath;
        private readonly ConfigurationSerializer _configurationSerializer;

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
        }

        public IConfiguration Configuration { get; }

        public IPipeline Pipeline { get; }

        public IPluginRepository PluginRepository { get; }

        public IProjectProvider ProjectProvider { get; }

        public event EventHandler<PipelineUpdateEventArgs> PipelineUpdated;

        public Task Update()
        {
            return Pipeline.Update();
        }

        public void PersistConfigurationChanges()
        {
            _configurationSerializer.Save(Configuration, _configFilePath);
        }

        private IConfiguration LoadConfiguration(string configFilePath)
        {
            return _configurationSerializer.Load(configFilePath);
        }

        private void Notifier_Updated(object sender, PipelineUpdateEventArgs e)
        {
            PipelineUpdated?.Invoke(this, e);
        }
    }
}