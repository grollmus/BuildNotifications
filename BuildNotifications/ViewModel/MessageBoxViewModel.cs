using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using BuildNotifications.Core.Text;
using BuildNotifications.ViewModel.Utils;

namespace BuildNotifications.ViewModel
{
    internal interface IRequestClose
    {
        event EventHandler CloseRequested;
    }

    internal class MessageBoxViewModel : PopupViewModel
    {
        public MessageBoxViewModel(string text, string title, MessageBoxButton buttons, MessageBoxImage icon, MessageBoxResult defaultResult)
        {
            Message = text;
            Title = title;
            Icon = icon;
            Result = defaultResult;

            Commands = new ObservableCollection<MessageBoxCommandViewModel>(ConstructCommands(buttons, defaultResult));
            CloseCommand = new DelegateCommand<MessageBoxResult>(Close);
        }

        public ICommand CloseCommand { get; }
        public IEnumerable<MessageBoxCommandViewModel> Commands { get; }
        public MessageBoxImage Icon { get; }
        public string Message { get; }
        public MessageBoxResult Result { get; private set; }
        public string Title { get; }

        private void Close(MessageBoxResult result)
        {
            Result = result;
            RequestClose();
        }

        private IEnumerable<MessageBoxCommandViewModel> ConstructCommands(MessageBoxButton buttons, MessageBoxResult defaultResult)
        {
            if (HasButton(buttons, MessageBoxResult.OK))
                yield return new MessageBoxCommandViewModel(StringLocalizer.Ok, MessageBoxResult.OK, defaultResult == MessageBoxResult.OK, false);

            if (HasButton(buttons, MessageBoxResult.Cancel))
                yield return new MessageBoxCommandViewModel(StringLocalizer.Cancel, MessageBoxResult.Cancel, defaultResult == MessageBoxResult.Cancel, true);

            if (HasButton(buttons, MessageBoxResult.Yes))
                yield return new MessageBoxCommandViewModel(StringLocalizer.Yes, MessageBoxResult.Yes, defaultResult == MessageBoxResult.Yes, false);

            if (HasButton(buttons, MessageBoxResult.No))
                yield return new MessageBoxCommandViewModel(StringLocalizer.No, MessageBoxResult.No, defaultResult == MessageBoxResult.No, true);
        }

        private bool HasButton(MessageBoxButton buttons, MessageBoxResult result)
        {
            return result switch
            {
                MessageBoxResult.No => (buttons == MessageBoxButton.YesNo || buttons == MessageBoxButton.YesNoCancel),
                MessageBoxResult.OK => (buttons == MessageBoxButton.OK || buttons == MessageBoxButton.OKCancel),
                MessageBoxResult.Yes => (buttons == MessageBoxButton.YesNo || buttons == MessageBoxButton.YesNoCancel),
                MessageBoxResult.Cancel => (buttons == MessageBoxButton.OKCancel || buttons == MessageBoxButton.YesNoCancel),
                _ => false
            };
        }
    }

    internal class MessageBoxCommandViewModel : BaseViewModel
    {
        public MessageBoxCommandViewModel(string text, MessageBoxResult result, bool isDefault, bool isCancel)
        {
            Text = text;
            Result = result;
            IsDefault = isDefault;
            IsCancel = isCancel;
        }

        public bool IsCancel { get; }
        public bool IsDefault { get; }

        public MessageBoxResult Result { get; }
        public string Text { get; }
    }
}