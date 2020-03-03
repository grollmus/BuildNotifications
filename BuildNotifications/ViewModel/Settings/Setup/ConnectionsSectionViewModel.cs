using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using BuildNotifications.Core.Config;
using BuildNotifications.Core.Plugin;
using BuildNotifications.Core.Text;
using BuildNotifications.Resources.Icons;
using BuildNotifications.ViewModel.Utils;

namespace BuildNotifications.ViewModel.Settings.Setup
{
    internal class ConnectionsSectionViewModel : SetupSectionViewModel
    {
        public ConnectionsSectionViewModel(IConfiguration configuration, IPluginRepository pluginRepository, Action saveAction)
            : base(configuration, saveAction)
        {
            _configuration = configuration;
            _pluginRepository = pluginRepository;

            Connections = new ObservableCollection<ConnectionViewModel>(
                ConstructConnectionViewModels(configuration, pluginRepository)
            );

            SelectedConnection = Connections.FirstOrDefault();

            AddConnectionCommand = new DelegateCommand(AddConnection);
            RemoveConnectionCommand = new DelegateCommand<ConnectionViewModel>(RemoveConnection);
        }

        public ICommand AddConnectionCommand { get; }

        public ObservableCollection<ConnectionViewModel> Connections { get; }

        public override string DisplayNameTextId => StringLocalizer.Keys.Connections;
        public override IconType Icon => IconType.Connection;

        public ICommand RemoveConnectionCommand { get; }

        public ConnectionViewModel? SelectedConnection
        {
            get => _selectedConnection;
            set
            {
                if (_selectedConnection == value)
                    return;

                if (_selectedConnection != null)
                    _selectedConnection.TestConnection.TestFinished -= TestConnection_TestFinished;

                _selectedConnection = value;
                OnPropertyChanged();

                if (_selectedConnection != null)
                    _selectedConnection.TestConnection.TestFinished += TestConnection_TestFinished;
            }
        }

        public event EventHandler? TestFinished;

        private void AddConnection()
        {
            var connection = new ConnectionData
            {
                Name = StringLocalizer.NewConnection
            };

            _configuration.Connections.Add(connection);

            var vm = new ConnectionViewModel(connection, _pluginRepository);
            vm.SaveRequested += ConnectionViewModel_SaveRequested;
            Connections.Add(vm);

            SelectedConnection = vm;
        }

        private void ConnectionViewModel_SaveRequested(object? sender, EventArgs e)
        {
            SaveAction.Invoke();
        }

        private IEnumerable<ConnectionViewModel> ConstructConnectionViewModels(IConfiguration configuration, IPluginRepository pluginRepository)
        {
            foreach (var c in configuration.Connections)
            {
                var connectionViewModel = new ConnectionViewModel(c, pluginRepository);
                connectionViewModel.SaveRequested += ConnectionViewModel_SaveRequested;
                yield return connectionViewModel;
            }
        }

        private void RaiseTestFinished()
        {
            TestFinished?.Invoke(this, EventArgs.Empty);
        }

        private void RemoveConnection(ConnectionViewModel viewModel)
        {
            viewModel.SaveRequested -= ConnectionViewModel_SaveRequested;
            _configuration.Connections.Remove(viewModel.Model);
            Connections.Remove(viewModel);
            SelectedConnection = Connections.FirstOrDefault();
        }

        private void TestConnection_TestFinished(object? sender, EventArgs e)
        {
            RaiseTestFinished();
        }

        private readonly IConfiguration _configuration;
        private readonly IPluginRepository _pluginRepository;
        private ConnectionViewModel? _selectedConnection;
    }
}