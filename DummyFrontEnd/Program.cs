using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuildNotifications.Core.Config;
using BuildNotifications.Core.Pipeline;
using BuildNotifications.Core.Plugin;
using BuildNotifications.Core.Plugin.Host;
using BuildNotifications.Core.Utilities;
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
            
            //var serializer = new Serializer();
            //var configSerializer = new ConfigurationSerializer(serializer);
            //var config = configSerializer.Load("../../../config.json");

            //var projectFactory = new ProjectFactory(pluginRepo, config);
            //var project = projectFactory.Construct(config.Projects.First());

            //var buildDefinitions = await ToListAsync(project.BuildProvider.FetchExistingBuildDefinitions());
            //var branches = await ToListAsync(project.BranchProvider.FetchExistingBranches());
            //var builds = await ToListAsync(project.BuildProvider.FetchAllBuilds());
            //var buildsToday = await ToListAsync(project.BuildProvider.FetchBuildsSince(DateTime.Today));
            //var buildsForDefinition = await ToListAsync(project.BuildProvider.FetchBuildsForDefinition(buildDefinitions.First()));
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