using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using BuildNotifications.Plugin.DummyBuildServer;

namespace DummyBuildServer.ViewModels
{
    internal class BuildDefinitionListViewModel : ViewModelBase
    {
        public BuildDefinitionListViewModel(MainViewModel mainViewModel, List<BuildDefinition> definitions)
        {
            _mainViewModel = mainViewModel;

            foreach (var definition in definitions)
            {
                Definitions.Add(new BuildDefinitionViewModel(definition));
            }

            AddDefinitionCommand = new DelegateCommand(AddDefinition, IsNameFilled);
            RemoveDefinitionCommand = new DelegateCommand(RemoveDefinition, IsDefinitionSelected);
        }

        public ICommand AddDefinitionCommand { get; }

        public string? DefinitionName
        {
            get => _definitionName;
            set
            {
                if (value == _definitionName)
                    return;
                _definitionName = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<BuildDefinitionViewModel> Definitions { get; } = new ObservableCollection<BuildDefinitionViewModel>();
        public ICommand RemoveDefinitionCommand { get; }

        public BuildDefinitionViewModel? SelectedDefinition
        {
            get => _selectedDefinition;
            set
            {
                if (Equals(value, _selectedDefinition))
                    return;
                _selectedDefinition = value;
                OnPropertyChanged();
            }
        }

        private void AddDefinition(object obj)
        {
            var def = new BuildDefinition(DefinitionName!);
            _mainViewModel.AddDefinition(def);
            Definitions.Add(new BuildDefinitionViewModel(def));

            SelectedDefinition = null;
            DefinitionName = string.Empty;
        }

        private bool IsDefinitionSelected(object arg)
        {
            return SelectedDefinition != null;
        }

        private bool IsNameFilled(object arg)
        {
            return !string.IsNullOrWhiteSpace(DefinitionName);
        }

        private void RemoveDefinition(object obj)
        {
            _mainViewModel.RemoveDefinition(SelectedDefinition!.Definition);
            Definitions.Remove(SelectedDefinition!);

            SelectedDefinition = null;
            DefinitionName = string.Empty;
        }

        private readonly MainViewModel _mainViewModel;
        private string? _definitionName;
        private BuildDefinitionViewModel? _selectedDefinition;
    }
}