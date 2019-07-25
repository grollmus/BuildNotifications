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
            if (!_configuration.Projects.Any())
                yield break;

            var project = _projectFactory.Construct(_configuration.Projects.First());
            yield return project;
        }

        private readonly IConfiguration _configuration;
        private readonly ProjectFactory _projectFactory;
    }

    public class CoreSetup
    {
        private readonly string _configFilePath;
        private readonly ConfigurationSerializer _configurationSerializer;

        public CoreSetup(string configFilePath)
        {
            _configFilePath = configFilePath;
            var serializer = new Serializer();
            _configurationSerializer = new ConfigurationSerializer(serializer);
            Configuration = _configurationSerializer.Load(configFilePath);

            var pluginLoader = new PluginLoader();
            PluginRepository = pluginLoader.LoadPlugins(new[] {"../../../plugins"});

            ProjectProvider = new ProjectProvider(Configuration, PluginRepository);

            var treeBuilder = new TreeBuilder(Configuration);
            Pipeline = new Pipeline.Pipeline(treeBuilder, Configuration);

            Pipeline.Notifier.Updated += Notifier_Updated;
        }

        public void PersistConfigurationChanges()
        {
            _configurationSerializer.Save(Configuration, _configFilePath);
        }

        public IConfiguration Configuration { get; }
        public IPipeline Pipeline { get; }
        public IPluginRepository PluginRepository { get; }
        public IProjectProvider ProjectProvider { get; }

        public event EventHandler<PipelineUpdateEventArgs> PipelineUpdated;

        //public IConfiguration DefaultConfiguration()
        //{
        //    var config = new Configuration();

        //    config.Connections.Add(new ConnectionData
        //    {
        //        Name = "LocalDummy",
        //        BuildPluginType = "BuildNotifications.Plugin.DummyBuildServer.Plugin",
        //        SourceControlPluginType = "BuildNotifications.Plugin.DummyBuildServer.Plugin",
        //        Options = new Dictionary<string, string>
        //        {
        //            {"port", "1111"}
        //        }
        //    });
        //    config.Projects.Add(new ProjectConfiguration { ProjectName = "Projecto", BuildConnectionName = "LocalDummy", SourceControlConnectionName = "LocalDummy"});

        //    return config;
        //}
        
        private void Notifier_Updated(object sender, PipelineUpdateEventArgs e)
        {
            PipelineUpdated?.Invoke(this, e);
        }

        public Task Update() => Pipeline.Update();
    }
}