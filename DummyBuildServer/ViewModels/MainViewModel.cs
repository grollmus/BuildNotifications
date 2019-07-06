using System.Windows.Input;
using BuildNotifications.Plugin.DummyBuildServer;
using DummyBuildServer.Models;

namespace DummyBuildServer.ViewModels
{
    internal class MainViewModel : ViewModelBase
    {
        public MainViewModel(DataSerializer serializer)
        {
            _serializer = serializer;

            _config = _serializer.Load(ServerConstants.DataFileName);

            Users = new UserListViewModel(this, _config.Users);
            BuildDefinitions = new BuildDefinitionListViewModel(this, _config.BuildDefinitions);
            Branches = new BranchListViewModel(this, _config.Branches);
            Builds = new BuildListViewModel(this);

            _server = new Server(_config);

            StartServerCommand = new DelegateCommand(StartServer, IsServerStopped);
            StopServerCommand = new DelegateCommand(StopServer, IsServerRunning);
        }

        public BranchListViewModel Branches { get; }
        public BuildDefinitionListViewModel BuildDefinitions { get; }
        public BuildListViewModel Builds { get; }
        public int Port { get; set; } = 1111;

        public ICommand StartServerCommand { get; }
        public ICommand StopServerCommand { get; }
        public UserListViewModel Users { get; }

        public void AddBranch(Branch branch)
        {
            _config.Branches.Add(branch);
            SaveData();
        }

        public void AddBuild(Build build)
        {
            _server.Builds.Add(build);
        }

        public void AddDefinition(BuildDefinition buildDefinition)
        {
            _config.BuildDefinitions.Add(buildDefinition);
            SaveData();
        }

        public void AddUser(User user)
        {
            _config.Users.Add(user);
            SaveData();
        }

        public void RemoveBranch(Branch branch)
        {
            _config.Branches.Remove(branch);
            SaveData();
        }

        public void RemoveDefinition(BuildDefinition buildDefinition)
        {
            _config.BuildDefinitions.Remove(buildDefinition);
            SaveData();
        }

        public void RemoveUser(User user)
        {
            _config.Users.Remove(user);
            SaveData();
        }

        public void UpdateBuild(Build build)
        {
            // TODO: Implement
        }

        private bool IsServerRunning(object arg)
        {
            return _server.IsRunning;
        }

        private bool IsServerStopped(object arg)
        {
            return !_server.IsRunning;
        }

        private void SaveData()
        {
            _serializer.Save(_config, ServerConstants.DataFileName);
        }

        private void StartServer(object arg)
        {
            _server.Start(Port);
        }

        private void StopServer(object arg)
        {
            _server.Stop();
        }

        private readonly DataSerializer _serializer;
        private readonly ServerConfig _config;
        private readonly Server _server;

        public void RemoveBuild(Build build)
        {
            _server.Builds.Remove(build);
        }
    }
}