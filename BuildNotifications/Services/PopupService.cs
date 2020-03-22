using System.Windows;

namespace BuildNotifications.Services
{
    internal class PopupService : IPopupService
    {
        public MessageBoxResult ShowMessageBox(string text, string title, MessageBoxButton buttons, MessageBoxImage icon = MessageBoxImage.Asterisk, MessageBoxResult defaultResult = MessageBoxResult.None)
        {
            var owner = Application.Current.MainWindow;
            if (owner != null)
                return MessageBox.Show(owner, text, title, buttons, icon, defaultResult);

            return MessageBox.Show(text, title, buttons, icon, defaultResult);
        }
    }
}