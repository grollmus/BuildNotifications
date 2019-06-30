using DummyBuildServer.Models;

namespace DummyBuildServer.ViewModels
{
    internal class MainViewModel : ViewModelBase
    {
        public MainViewModel(DataSerializer serializer)
        {
            _serializer = serializer;

            _config = _serializer.Load(Constants.DataFileName);

            Users = new UserListViewModel(this, _config.Users);
            BuildDefinitions = new BuildDefinitionListViewModel(this, _config.BuildDefinitions);
            Branches = new BranchListViewModel(this, _config.Branches);
            Builds = new BuildListViewModel(this);
        }

        public BranchListViewModel Branches { get; }
        public BuildDefinitionListViewModel BuildDefinitions { get; }
        public BuildListViewModel Builds { get; }
        public UserListViewModel Users { get; }

        public void AddBranch(Branch branch)
        {
            _config.Branches.Add(branch);
            SaveData();
        }

        public void AddBuild(Build build)
        {
            // TODO: Implement
        }

        public void UpdateBuild(Build build)
        {
            // TODO: Implement
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

        private void SaveData()
        {
            _serializer.Save(_config, Constants.DataFileName);
        }

        private readonly DataSerializer _serializer;
        private readonly ServerConfig _config;
    }
}