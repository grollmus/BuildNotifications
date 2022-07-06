using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using BuildNotifications.Core.Config;
using BuildNotifications.Core.Text;
using BuildNotifications.Resources.Icons;
using BuildNotifications.Services;
using BuildNotifications.ViewModel.Utils;

namespace BuildNotifications.ViewModel.Settings.Setup;

internal class ProjectsSectionViewModel : SetupSectionViewModel
{
    public ProjectsSectionViewModel(IConfigurationBuilder configurationBuilder, IConfiguration configuration, Action saveAction, IPopupService popupService)
        : base(configuration, saveAction, popupService)
    {
        _configurationBuilder = configurationBuilder;
        _configuration = configuration;

        Projects = new ObservableCollection<ProjectViewModel>(
            ConstructProjectViewModels(configuration)
        );

        SelectedProject = Projects.FirstOrDefault();

        AddProjectCommand = new DelegateCommand(AddProject);
        RemoveProjectCommand = new DelegateCommand<ProjectViewModel>(RemoveProject);
    }

    public ICommand AddProjectCommand { get; }

    public override string DisplayNameTextId => StringLocalizer.Keys.Projects;
    public override IconType Icon => IconType.Project;

    public ObservableCollection<ProjectViewModel> Projects { get; }
    public ICommand RemoveProjectCommand { get; }

    public ProjectViewModel? SelectedProject
    {
        get => _selectedProject;
        set
        {
            if (_selectedProject == value)
                return;

            _selectedProject = value;
            OnPropertyChanged();
        }
    }

    public override void Refresh()
    {
        foreach (var project in Projects)
        {
            project.RefreshConnections();
        }
    }

    internal void AddProjectViewModel(ProjectViewModel vm)
    {
        vm.SaveRequested += ProjectViewModel_SaveRequested;
        Projects.Add(vm);

        SelectedProject = vm;
    }

    private void AddProject()
    {
        var project = _configurationBuilder.EmptyConfiguration(StringLocalizer.NewProject);

        _configuration.Projects.Add(project);

        var vm = new ProjectViewModel(project, _configuration);
        AddProjectViewModel(vm);
    }

    private IEnumerable<ProjectViewModel> ConstructProjectViewModels(IConfiguration configuration)
    {
        foreach (var p in configuration.Projects)
        {
            var projectViewModel = new ProjectViewModel(p, _configuration);
            projectViewModel.SaveRequested += ProjectViewModel_SaveRequested;
            yield return projectViewModel;
        }
    }

    private void ProjectViewModel_SaveRequested(object? sender, EventArgs e)
    {
        SaveAction.Invoke();
    }

    private void RemoveProject(ProjectViewModel viewModel)
    {
        var text = string.Format(StringLocalizer.CurrentCulture, StringLocalizer.ConfirmDeleteProject, viewModel.Name.Value);
        var confirm = PopupService.ShowMessageBox(text, StringLocalizer.ConfirmDeletion, MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
        if (confirm != MessageBoxResult.Yes)
            return;

        viewModel.SaveRequested -= ProjectViewModel_SaveRequested;
        _configuration.Projects.Remove(viewModel.Model);
        Projects.Remove(viewModel);
        SelectedProject = Projects.FirstOrDefault();

        SaveAction.Invoke();
    }

    private readonly IConfigurationBuilder _configurationBuilder;
    private readonly IConfiguration _configuration;
    private ProjectViewModel? _selectedProject;
}