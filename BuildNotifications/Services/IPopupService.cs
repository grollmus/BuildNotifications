using System.Windows;
using BuildNotifications.Core.Config;

namespace BuildNotifications.Services
{
    internal interface IPopupService
    {
        string? AskForOpenFileName(params string[] fileTypes);
        string? AskForSaveFileName(params string[] fileTypes);

        void ShowImportExportPopup(IConfigurationService configurationService);

        void ShowInfoPopup(bool includePreReleases, IAppUpdater appUpdater);

        MessageBoxResult ShowMessageBox(string text, string title, MessageBoxButton buttons, MessageBoxImage icon = MessageBoxImage.Information, MessageBoxResult defaultResult = MessageBoxResult.None);
    }
}