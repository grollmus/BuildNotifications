using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using BuildNotifications.Core.Config;
using BuildNotifications.Core.Plugin;
using BuildNotifications.Core.Text;
using BuildNotifications.Resources.Icons;
using BuildNotifications.ViewModel.Utils;

namespace BuildNotifications.ViewModel.Settings
{
    internal class ConnectionsSectionViewModel : SetupSectionViewModel
    {
        public ConnectionsSectionViewModel(IConfiguration configuration, IPluginRepository pluginRepository)
            : base(configuration, pluginRepository)
        {
            _configuration = configuration;
            _pluginRepository = pluginRepository;

            Connections = new ObservableCollection<ConnectionViewModel>(
                configuration.Connections.Select(c => new ConnectionViewModel(c, pluginRepository))
            );

            SelectedConnection = Connections.FirstOrDefault();

            AddConnectionCommand = new DelegateCommand(AddConnection);
            RemoveConnectionCommand = new DelegateCommand<ConnectionViewModel>(RemoveConnection);
        }

        public ICommand AddConnectionCommand { get; }

        public ObservableCollection<ConnectionViewModel> Connections { get; }

        public override string DisplayNameTextId => StringLocalizer.Keys.Connections;
        public override IconType IconType => IconType.Connection;

        public ICommand RemoveConnectionCommand { get; }

        public ConnectionViewModel? SelectedConnection
        {
            get => _selectedConnection;
            set
            {
                if (_selectedConnection == value)
                    return;

                _selectedConnection = value;
                OnPropertyChanged();
            }
        }

        private void AddConnection()
        {
            var connection = new ConnectionData
            {
                Name = StringLocalizer.NewConnection
            };

            _configuration.Connections.Add(connection);

            var vm = new ConnectionViewModel(connection, _pluginRepository);
            Connections.Add(vm);

            SelectedConnection = vm;
        }

        private void RemoveConnection(ConnectionViewModel viewModel)
        {
            _configuration.Connections.Remove(viewModel.Model);
            Connections.Remove(viewModel);
            SelectedConnection = Connections.FirstOrDefault();
        }

        private readonly IConfiguration _configuration;
        private readonly IPluginRepository _pluginRepository;
        private ConnectionViewModel? _selectedConnection;
    }
}