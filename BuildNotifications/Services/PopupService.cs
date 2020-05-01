using System.Windows;
using BuildNotifications.ViewModel;
using BuildNotifications.Views;

namespace BuildNotifications.Services
{
    internal class PopupService : IPopupService
    {
        public PopupService(IBlurrableViewModel blur)
        {
            _blur = blur;
        }

        public MessageBoxResult ShowMessageBox(string text, string title, MessageBoxButton buttons, MessageBoxImage icon = MessageBoxImage.Asterisk, MessageBoxResult defaultResult = MessageBoxResult.None)
        {
            var vm = new MessageBoxViewModel(text, title, buttons, icon, defaultResult);

            var owner = Application.Current.MainWindow;
            var popup = new MessageBoxView
            {
                Owner = owner,
                DataContext = vm
            };

            _blur.Blur();
            popup.ShowDialog();
            _blur.UnBlur();

            return vm.Result;
        }

        private readonly IBlurrableViewModel _blur;
    }
}