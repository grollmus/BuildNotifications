using System;
using System.Collections.Generic;
using System.Linq;
using BuildNotifications.Plugin.DummyBuildServer;
using DummyServer.Models;

namespace DummyServer.Services
{
    internal class DataStorage : IDataStorage
    {
        private readonly ServerConfig _config;
        private readonly DataSerializer _serializer = new DataSerializer();

        private const string ConfigFileName = "serverConfig.json";

        private readonly List<Build> _builds = new List<Build>();

        public DataStorage()
        {
            _config = _serializer.Load(ConfigFileName);
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
            throw new NotImplementedException();
        }

        private T AccessConfig<T>(Func<ServerConfig, T> propertyAccessor)
        {
            lock (_config)
            {
                return propertyAccessor(_config);
            }
        }

        private void WriteConfig(Action<ServerConfig> action)
        {
            lock (_config)
            {
                action(_config);
                _serializer.Save(_config, ConfigFileName);
            }
        }

        public void SetBuild(List<Build> to)
        {
            lock (_builds)
            {
                _builds.Clear();
                _builds.AddRange(to);
            }
        }
    }
}