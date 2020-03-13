using System.IO;
using BuildNotifications.Plugin.DummyBuildServer;

namespace DummyBuildServer.Models
{
    internal class BuildCommandParser : ServerCommandParser
    {
        public BuildCommandParser(Server server)
            : base(server)
        {
            CommandMap[Constants.Queries.Definitions] = ListDefinitions;
            CommandMap[Constants.Queries.Builds] = ListBuilds;
        }

        private void ListBuilds(string command, Stream responseStream)
        {
            WriteObject(responseStream, Server.Builds);
        }

        private void ListDefinitions(string command, Stream response)
        {
            WriteObject(response, Server.Config.BuildDefinitions);
        }
    }
}