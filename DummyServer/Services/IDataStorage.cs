using System.Collections.Generic;
using BuildNotifications.Plugin.DummyBuildServer;

namespace DummyServer.Services
{
    public interface IDataStorage
    {
        List<Branch> Branches();

        List<BuildDefinition> BuildDefinitions();

        List<User> Users();

        List<Build> Builds();

        void AddBranch(string name);

        void DeleteBranch(string name);

        void AddDefinition(string name);
        void DeleteDefinition(string name);

        void AddUser(string name);
        void DeleteUser(string name);

        void AddBuild(string branchName, string definitionName, string userName);

        void PermutateBuilds();

        void RandomizeBuildStatus();

        void DeleteBuild(string id);
    }
}