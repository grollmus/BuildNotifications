using System.Windows;

namespace BuildNotifications.Services
{
    internal interface IPopupService
    {
        MessageBoxResult ShowMessageBox(string text, string title, MessageBoxButton buttons, MessageBoxImage icon = MessageBoxImage.Information, MessageBoxResult defaultResult = MessageBoxResult.None);
    }
}