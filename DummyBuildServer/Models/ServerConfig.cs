using System.Collections.Generic;

namespace DummyBuildServer.Models
{
    internal class ServerConfig
    {
        public ServerConfig()
        {
            Users = new List<User>();
            BuildDefinitions = new List<BuildDefinition>();
            Branches = new List<Branch>();
        }

        public List<Branch> Branches { get; set; }
        public List<BuildDefinition> BuildDefinitions { get; set; }
        public List<User> Users { get; set; }
    }
}