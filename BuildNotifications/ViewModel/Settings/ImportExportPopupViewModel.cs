using System.Windows;
using System.Windows.Input;
using BuildNotifications.Core.Config;
using BuildNotifications.Core.Text;
using BuildNotifications.Services;
using BuildNotifications.ViewModel.Utils;

namespace BuildNotifications.ViewModel.Settings
{
    internal class ImportExportPopupViewModel : PopupViewModel
    {
        public ImportExportPopupViewModel(IPopupService popupService, IConfigurationService configurationService)
        {
            _popupService = popupService;
            _configurationService = configurationService;

            ImportCommand = new DelegateCommand(Import);
            ExportCommand = new DelegateCommand(Export);
        }

        public ICommand ExportCommand { get; }

        public ICommand ImportCommand { get; }

        private void Export()
        {
            var fileName = _popupService.AskForSaveFileName(StringLocalizer.ConfigurationFileFilter);
            if (fileName == null)
                return;

            _configurationService.Serializer.Save(_configurationService.CurrentConfig, fileName);
            _popupService.ShowMessageBox(StringLocalizer.ExportSuccessful, StringLocalizer.Export, MessageBoxButton.OK);
            RequestClose();
        }

        private void Import()
        {
            var fileName = _popupService.AskForOpenFileName(StringLocalizer.ConfigurationFileFilter);
            if (fileName == null)
                return;

            var config = _configurationService.Serializer.Load(fileName);
            _configurationService.Merge(config);

            _popupService.ShowMessageBox(StringLocalizer.ImportSuccessful, StringLocalizer.Import, MessageBoxButton.OK);
            RequestClose();
        }

        private readonly IPopupService _popupService;
        private readonly IConfigurationService _configurationService;
    }
}