using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuildNotifications.Core.Config;
using BuildNotifications.Core.Pipeline;
using BuildNotifications.Core.Pipeline.Tree;
using BuildNotifications.Core.Plugin;
using BuildNotifications.Core.Utilities;

namespace BuildNotifications.Core
{
    public interface IProjectProvider
    {
        IEnumerable<IProject> AllProjects();
    }

    internal class ProjectProvider : IProjectProvider
    {
        public ProjectProvider(IConfiguration configuration, IPluginRepository pluginRepository)
        {
            _configuration = configuration;

            _projectFactory = new ProjectFactory(pluginRepository, configuration);
        }

        public IEnumerable<IProject> AllProjects()
        {
            var project = _projectFactory.Construct(_configuration.Projects.First());
            yield return project;
        }

        private readonly IConfiguration _configuration;
        private readonly ProjectFactory _projectFactory;
    }

    public class CoreSetup
    {
        public CoreSetup()
        {
            var serializer = new Serializer();
            Configuration = LoadConfiguration(serializer);

            var pluginLoader = new PluginLoader();
            PluginRepository = pluginLoader.LoadPlugins(new[] {"../../../plugins"});

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
            var configSerializer = new ConfigurationSerializer(serializer);

            IConfiguration config = new Configuration();

            config.Connections.Add(new ConnectionData
            {
                Name = "LocalDummy",
                BuildPluginType = "BuildNotifications.Plugin.DummyBuildServer.Plugin",
                SourceControlPluginType = "BuildNotifications.Plugin.DummyBuildServer.Plugin",
                Options = new Dictionary<string, string?>
                {
                    {"port", "1111"}
                }
            });

            config.Projects.Add(new ProjectConfiguration {ProjectName = "Projecto", BuildConnectionName = "LocalDummy", SourceControlConnectionName = "LocalDummy"});

            configSerializer.Save(config, "../../../config.json");
            config = configSerializer.Load("../../../config.json");
            return config;
        }

        private void Notifier_Updated(object sender, PipelineUpdateEventArgs e)
        {
            PipelineUpdated?.Invoke(this, e);
        }
    }
}