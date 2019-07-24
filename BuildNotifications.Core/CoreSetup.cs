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
        public CoreSetup()
        {
            var serializer = new Serializer();
            Configuration = LoadConfiguration(serializer);

            var pluginLoader = new PluginLoader();
            PluginRepository = pluginLoader.LoadPlugins(new[] {"plugins"});

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

        private static IConfiguration LoadConfiguration(ISerializer serializer)
        {
            const string configFilename = "config.json";

            var configSerializer = new ConfigurationSerializer(serializer);
            return configSerializer.Load(configFilename);
        }

        private void Notifier_Updated(object sender, PipelineUpdateEventArgs e)
        {
            PipelineUpdated?.Invoke(this, e);
        }
    }
}