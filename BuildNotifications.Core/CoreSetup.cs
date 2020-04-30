using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Anotar.NLog;
using BuildNotifications.Core.Config;
using BuildNotifications.Core.Pipeline;
using BuildNotifications.Core.Pipeline.Notification.Distribution;
using BuildNotifications.Core.Pipeline.Tree;
using BuildNotifications.Core.Pipeline.Tree.Search;
using BuildNotifications.Core.Pipeline.Tree.Search.Criteria;
using BuildNotifications.Core.Plugin;
using BuildNotifications.Core.Utilities;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfaces.Builds.Search;

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

            var searcher = new BuildSearcher();
            var treeBuilder = new TreeBuilder(Configuration, searcher);
            Pipeline = new Pipeline.Pipeline(treeBuilder, Configuration);

            Pipeline.Notifier.Updated += Notifier_Updated;

            SearchEngine = new SearchEngine();
            SearchEngine.AddCriteria(new BeforeCriteria());
            SearchEngine.AddCriteria(new TestSearchCriteria());
            SearchEngine.AddCriteria(new TestSearchCriteria2());

            if (notificationReceiver != null)
            {
                NotificationReceiver = notificationReceiver;
                NotificationReceiver.DistributedNotificationReceived += (sender, args) => DistributedNotificationReceived?.Invoke(this, args);
            }
        }

        public ISearchEngine SearchEngine { get; }

        public IConfiguration Configuration { get; }

        public IDistributedNotificationReceiver? NotificationReceiver { get; }

        public IPipeline Pipeline { get; }

        public IPluginRepository PluginRepository { get; }

        public IProjectProvider ProjectProvider { get; }

        public event EventHandler<DistributedNotificationReceivedEventArgs>? DistributedNotificationReceived;

        public event EventHandler<PipelineUpdateEventArgs>? PipelineUpdated;

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

    public class TestSearchCriteria : ISearchCriteria
    {
        public string LocalizedKeyword => "asd";

        public string LocalizedDescription => "Temporary criteria for GUI testing purposes";

        public IEnumerable<ISearchCriteriaSuggestion> Suggest(string input)
        {
            return Examples().Where(e => e.StartsWith(input, StringComparison.InvariantCultureIgnoreCase)).Select(e => new SearchCriteriaSuggestion(e));
        }

        public bool IsBuildIncluded(IBuild build, string input) => true;
        public IEnumerable<string> LocalizedExamples => Examples();

        private IEnumerable<string> Examples()
        {
            yield return "Test";
            yield return "Something";
            yield return "Hello";
        }
    }
    
    public class TestSearchCriteria2 : ISearchCriteria
    {
        public string LocalizedKeyword => "test";

        public string LocalizedDescription => "Temporary criteria for GUI testing purposes";

        public IEnumerable<ISearchCriteriaSuggestion> Suggest(string input)
        {
            return Examples().Where(e => e.StartsWith(input, StringComparison.InvariantCultureIgnoreCase)).Select(e => new SearchCriteriaSuggestion(e));
        }

        public bool IsBuildIncluded(IBuild build, string input) => true;
        public IEnumerable<string> LocalizedExamples => Examples();

        private IEnumerable<string> Examples()
        {
            yield return "Don't care";
            yield return "Anything";
            yield return "Absolutely anything";
        }
    }
}