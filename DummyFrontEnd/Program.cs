using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BuildNotifications.Core.Config;
using BuildNotifications.Core.Pipeline;
using BuildNotifications.Core.Pipeline.Tree;
using BuildNotifications.Core.Plugin;
using BuildNotifications.Core.Plugin.Host;
using BuildNotifications.Core.Utilities;
using BuildNotifications.PluginInterfaces.Builds;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace DummyFrontEnd
{
    internal class Program
    {
        private static async Task Main()
        {
            SetupLogging();

            var host = new PluginHost();

            var pluginLoader = new PluginLoader();
            var pluginRepo = pluginLoader.LoadPlugins(new[] {"../../../plugins"});

            var plugin = pluginRepo.Build.First();
            var schema = plugin.GetSchema(host);

            var serializer = new Serializer();
            var configSerializer = new ConfigurationSerializer(serializer);
            var config = configSerializer.Load("../../../config.json");
            //var config = new Configuration();

            //config.Connections.Add(new ConnectionData {Name = "LocalDummy", BuildPluginType = "BuildNotifications.Plugin.DummyBuildServer.Plugin", SourceControlPluginType = "BuildNotifications.Plugin.DummyBuildServer.Plugin", Options = new Dictionary<string, string>
            //{
            //    {"port", "1111" }
            //}});
            //config.Projects.Add(new ProjectConfiguration {BuildConnectionName = "LocalDummy", SourceControlConnectionName = "LocalDummy"});

            //configSerializer.Save(config, "../../../config.json");

            var treeBuilder = new TreeBuilder(config);
            var pipeline = new Pipeline(treeBuilder);

            Console.WriteLine("Waiting for dummy server to start");
            var tryCount = 0;
            while (tryCount < 60)
            {
                ++tryCount;

                try
                {
                    MemoryMappedFile.OpenExisting("BuildNotifications.DummyBuildServer");
                    break;
                }
                catch (Exception)
                {
                    Thread.Sleep(1000);
                }
            }

            Console.WriteLine("Dummy server running");

            var projectFactory = new ProjectFactory(pluginRepo, config);
            var project = projectFactory.Construct(config.Projects.First());

            pipeline.AddProject(project);

            pipeline.Notifier.Updated += Notifier_Updated;

            while (!Console.KeyAvailable)
            {
                Console.WriteLine("Start Pipeline Update");
                await pipeline.Update();
                Console.WriteLine("Finish Pipeline Update");

                Console.WriteLine("Sleeping 30 seconds...");
                Thread.Sleep(TimeSpan.FromSeconds(30));
            }

            //var buildDefinitions = await ToListAsync(project.BuildProvider.FetchExistingBuildDefinitions());
            //var branches = await ToListAsync(project.BranchProvider.FetchExistingBranches());
            //var builds = await ToListAsync(project.BuildProvider.FetchAllBuilds());
            //var buildsToday = await ToListAsync(project.BuildProvider.FetchBuildsSince(DateTime.Today));
            //var buildsForDefinition = await ToListAsync(project.BuildProvider.FetchBuildsForDefinition(buildDefinitions.First()));
        }

        private static void Notifier_Updated(object sender, PipelineUpdateEventArgs e)
        {
            var childrenCount = e.Tree.Children.Count();
            Console.WriteLine($"Children in tree: {childrenCount}");

            var builds = e.Tree.Children.OfType<IBuildNode>().ToList();
            Console.WriteLine($"Builds in tree: {childrenCount}");

            var nonCompleted = builds.Where(b => b.Build.Status == BuildStatus.Pending).ToList();
            Console.WriteLine($"Non completed builds: {nonCompleted.Count()}");

            Console.WriteLine(string.Join(", ", nonCompleted.Select(b => b.Build.Id)));
        }

        private static void SetupLogging()
        {
            var config = new LoggingConfiguration();
            var debuggerTarget = new DebuggerTarget
            {
                Layout = "[${level:uppercase=true}] ${logger}: ${message} ${exception}"
            };
            config.AddTarget("debugger", debuggerTarget);

            var debuggerRule = new LoggingRule("*", LogLevel.Trace, debuggerTarget);
            config.LoggingRules.Add(debuggerRule);

            LogManager.Configuration = config;
        }

        private static async Task<List<T>> ToListAsync<T>(IAsyncEnumerable<T> source)
        {
            var result = new List<T>();

            await foreach (var item in source)
            {
                result.Add(item);
            }

            return result;
        }
    }
}