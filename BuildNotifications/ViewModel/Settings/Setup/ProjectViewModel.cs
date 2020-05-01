using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using BuildNotifications.Core.Config;
using BuildNotifications.Core.Text;
using BuildNotifications.Resources.Global.Navigation.ButtonNavigation;
using BuildNotifications.Resources.Icons;
using BuildNotifications.ViewModel.Settings.Options;
using BuildNotifications.ViewModel.Utils;

namespace BuildNotifications.ViewModel.Settings.Setup
{
    internal class ProjectViewModel : BaseButtonNavigationItem
    {
        public ProjectViewModel(IProjectConfiguration model, IConfiguration configuration)
        {
            Model = model;
            SaveCommand = new DelegateCommand(Save);

            Name = new TextOptionViewModel(model.ProjectName, StringLocalizer.ProjectName);
            IsEnabled = new BooleanOptionViewModel(model.IsEnabled, StringLocalizer.IsEnabled);
            PullRequestDisplayMode = new PullRequestDisplayModeOptionViewModel(StringLocalizer.ShowPullRequests, model.PullRequestDisplay);
            DefaultCompareBranch = new TextOptionViewModel(model.DefaultCompareBranch, StringLocalizer.DefaultCompareBranch);
            HideCompletedPullRequests = new BooleanOptionViewModel(model.HideCompletedPullRequests, StringLocalizer.HideCompletedPullRequests);

            BranchBlacklist = new StringCollectionOptionViewModel(StringLocalizer.BranchBlacklist, model.BranchBlacklist);
            BranchWhitelist = new StringCollectionOptionViewModel(StringLocalizer.BranchWhitelist, model.BranchWhitelist);
            BuildDefinitionBlacklist = new StringCollectionOptionViewModel(StringLocalizer.BuildDefinitionBlacklist, model.BuildDefinitionBlacklist);
            BuildDefinitionWhitelist = new StringCollectionOptionViewModel(StringLocalizer.BuildDefinitionWhitelist, model.BuildDefinitionWhitelist);

            var connections = configuration.Connections.Where(c => c.ConnectionType == ConnectionPluginType.SourceControl).ToList();
            var connection = connections.FirstOrDefault(c => c.Name == model.SourceControlConnectionName);
            SourceControlConnection = new ConnectionOptionViewModel(StringLocalizer.SourceControlConnectionNames, connections, connection);

            connections = configuration.Connections.Where(c => c.ConnectionType == ConnectionPluginType.Build).ToList();
            connection = connections.FirstOrDefault(c => c.Name == model.BuildConnectionName.FirstOrDefault());
            BuildConnection = new ConnectionOptionViewModel(StringLocalizer.BuildConnectionNames, connections, connection);

            foreach (var option in Options)
            {
                option.ValueChanged += Option_ValueChanged;
            }
        }

        public StringCollectionOptionViewModel BranchBlacklist { get; }
        public StringCollectionOptionViewModel BranchWhitelist { get; }
        public ConnectionOptionViewModel BuildConnection { get; }
        public StringCollectionOptionViewModel BuildDefinitionBlacklist { get; }
        public StringCollectionOptionViewModel BuildDefinitionWhitelist { get; }
        public TextOptionViewModel DefaultCompareBranch { get; }
        public override string DisplayNameTextId => Model.ProjectName;
        public BooleanOptionViewModel HideCompletedPullRequests { get; }
        public override IconType Icon => IconType.None;
        public BooleanOptionViewModel IsEnabled { get; }
        public IProjectConfiguration Model { get; }
        public TextOptionViewModel Name { get; }

        public IEnumerable<OptionViewModelBase> Options
        {
            get
            {
                yield return IsEnabled;
                yield return Name;
                yield return BuildConnection;
                yield return SourceControlConnection;
                yield return BranchBlacklist;
                yield return BranchWhitelist;
                yield return BuildDefinitionBlacklist;
                yield return BuildDefinitionWhitelist;
                yield return DefaultCompareBranch;
                yield return HideCompletedPullRequests;
                yield return PullRequestDisplayMode;
            }
        }

        public PullRequestDisplayModeOptionViewModel PullRequestDisplayMode { get; }

        public ICommand SaveCommand { get; }
        public ConnectionOptionViewModel SourceControlConnection { get; }
        public virtual event EventHandler<EventArgs>? SaveRequested;

        private void Option_ValueChanged(object? sender, EventArgs e)
        {
            Save();
        }

        private void RaiseSaveRequested()
        {
            SaveRequested?.Invoke(this, EventArgs.Empty);
        }

        private void Save()
        {
            Model.ProjectName = Name.Value ?? string.Empty;
            Model.BuildConnectionName.Clear();
            Model.BuildConnectionName.Add(BuildConnection.Value.Name);
            Model.SourceControlConnectionName = SourceControlConnection.Value.Name;
            Model.IsEnabled = IsEnabled.Value;
            Model.HideCompletedPullRequests = HideCompletedPullRequests.Value;
            Model.DefaultCompareBranch = DefaultCompareBranch.Value ?? string.Empty;
            Model.BranchWhitelist = BranchWhitelist.Values.Where(v => v.Value != null).Select(v => v.Value!).ToList();
            Model.BranchBlacklist = BranchBlacklist.Values.Where(v => v.Value != null).Select(v => v.Value!).ToList();
            Model.BuildDefinitionWhitelist = BuildDefinitionWhitelist.Values.Where(v => v.Value != null).Select(v => v.Value!).ToList();
            Model.BuildDefinitionBlacklist = BuildDefinitionBlacklist.Values.Where(v => v.Value != null).Select(v => v.Value!).ToList();

            RaiseSaveRequested();
        }
    }
}