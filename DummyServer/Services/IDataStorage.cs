using System.Collections.Generic;
using BuildNotifications.Plugin.DummyServer;

namespace DummyServer.Services;

public interface IDataStorage
{
    void AddBranch(string name);

    void AddBuild(string branchName, string definitionName, string userName);

    void AddDefinition(string name);

    void AddUser(string name);
    List<Branch> Branches();

    List<BuildDefinition> BuildDefinitions();

    List<Build> Builds();

    void DeleteBranch(string name);

    void DeleteBuild(string id);
    void DeleteDefinition(string name);
    void DeleteUser(string name);

    void PermutateBuilds();

    void RandomizeBuildStatus();

    List<User> Users();
}