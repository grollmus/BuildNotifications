using System;
using System.Collections.Generic;
using System.Linq;
using BuildNotifications.Plugin.DummyServer;
using BuildNotifications.PluginInterfaces.Builds;
using DummyServer.Models;

namespace DummyServer.Services;

internal class DataStorage : IDataStorage
{
    public DataStorage()
    {
        _config = _serializer.Load(ConfigFileName);
    }

    public void SetBuild(List<Build> to)
    {
        lock (_builds)
        {
            _builds.Clear();
            _builds.AddRange(to);
        }
    }

    private T AccessConfig<T>(Func<ServerConfig, T> propertyAccessor)
    {
        lock (_config)
        {
            return propertyAccessor(_config);
        }
    }

    private Build ConstructBuild(string branchName, string definitionName, string userName)
    {
        var definition = BuildDefinitions().FirstOrDefault(d => d.Name.Equals(definitionName));

        if (definition == null)
            definition = new BuildDefinition(definitionName);

        var build = new Build();

        build.BranchName = branchName;
        build.Definition = definition;
        var user = new User(userName);
        build.RequestedBy = user;
        build.RequestedFor = user;
        build.Status = BuildStatus.Pending;
        build.LastChangedTime = DateTime.Now;
        build.QueueTime = DateTime.Now;
        build.Reason = RandomEnum<BuildReason>();
        return build;
    }

    private static T RandomEnum<T>()
    {
        var reasonValues = Enum.GetValues(typeof(T));
        var randomIndex = _random.Next(1, reasonValues.Length);
        var value = reasonValues.GetValue(randomIndex);
        if (value is T asT)
            return asT;

        return default;
    }

    private void WriteConfig(Action<ServerConfig> action)
    {
        lock (_config)
        {
            action(_config);
            _serializer.Save(_config, ConfigFileName);
        }
    }

    public List<Branch> Branches() => AccessConfig(c => c.Branches);

    public List<BuildDefinition> BuildDefinitions() => AccessConfig(c => c.BuildDefinitions);

    public List<User> Users() => AccessConfig(c => c.Users);

    public List<Build> Builds()
    {
        lock (_builds)
        {
            return _builds.ToList();
        }
    }

    public void AddBranch(string name)
    {
        if (string.IsNullOrWhiteSpace(name) || AccessConfig(c => c.Branches.Any(b => b.FullName.Equals(name))))
            return;

        WriteConfig(c => c.Branches.Add(new Branch(name)));
    }

    public void DeleteBranch(string name) => WriteConfig(c =>
    {
        name ??= string.Empty;

        var branchToDelete = c.Branches.FirstOrDefault(b => b.FullName == name);
        if (branchToDelete != null)
            c.Branches.Remove(branchToDelete);
    });

    public void AddDefinition(string name)
    {
        if (string.IsNullOrWhiteSpace(name) || AccessConfig(c => c.BuildDefinitions.Any(b => b.Name.Equals(name))))
            return;

        WriteConfig(c => c.BuildDefinitions.Add(new BuildDefinition(name)));
    }

    public void DeleteDefinition(string name) => WriteConfig(c =>
    {
        name ??= string.Empty;

        var definitionToDelete = c.BuildDefinitions.FirstOrDefault(b => b.Name == name);
        if (definitionToDelete != null)
            c.BuildDefinitions.Remove(definitionToDelete);
    });

    public void AddUser(string name)
    {
        if (string.IsNullOrWhiteSpace(name) || AccessConfig(c => c.Users.Any(b => b.UniqueName.Equals(name))))
            return;

        WriteConfig(c => c.Users.Add(new User(name)));
    }

    public void DeleteUser(string name) => WriteConfig(c =>
    {
        name ??= string.Empty;

        var userToDelete = c.Users.FirstOrDefault(b => b.UniqueName == name);
        if (userToDelete != null)
            c.Users.Remove(userToDelete);
    });

    public void AddBuild(string branchName, string definitionName, string userName)
    {
        var build = ConstructBuild(branchName, definitionName, userName);

        lock (_builds)
        {
            _builds.Add(build);
        }
    }

    public void PermutateBuilds()
    {
        var branches = Branches();
        var definitions = BuildDefinitions();
        var users = Users();

        var builds = new List<Build>();

        foreach (var branch in branches)
        {
            foreach (var definition in definitions)
            {
                foreach (var user in users)
                {
                    builds.Add(ConstructBuild(branch.FullName, definition.Name, user.UniqueName));
                }
            }
        }

        lock (_builds)
        {
            _builds.AddRange(builds);
        }
    }

    public void RandomizeBuildStatus()
    {
        lock (_builds)
        {
            foreach (var build in _builds)
            {
                build.Status = RandomEnum<BuildStatus>();
            }
        }
    }

    public void DeleteBuild(string id)
    {
        lock (_builds)
        {
            var existingBuild = _builds.FirstOrDefault(b => b.Id.Equals(id));
            if (existingBuild == null)
                return;
            _builds.Remove(existingBuild);
        }
    }

    private readonly ServerConfig _config;
    private readonly DataSerializer _serializer = new();

    private readonly List<Build> _builds = new();

    private const string ConfigFileName = "serverConfig.json";

    private static readonly Random _random = new();
}