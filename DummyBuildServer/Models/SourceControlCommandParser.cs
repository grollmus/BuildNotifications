using System.IO;
using BuildNotifications.Plugin.DummyBuildServer;

namespace DummyBuildServer.Models
{
    internal class SourceControlCommandParser : ServerCommandParser
    {
        public SourceControlCommandParser(Server server)
            : base(server)
        {
            CommandMap[Constants.Queries.Branches] = ListBranches;
        }

        private void ListBranches(string args, Stream response)
        {
            WriteObject(response, Server.Config.Branches);
        }
    }
}