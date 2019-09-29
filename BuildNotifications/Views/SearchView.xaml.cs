using System.Windows;
using System.Windows.Input;
using BuildNotifications.ViewModel.Utils;

namespace BuildNotifications.Views
{
    public partial class SearchView
    {
        public SearchView()
        {
            InitializeComponent();
            FocusSearchBarCommand = new DelegateCommand(FocusSearchBar);

            if (Application.Current.MainWindow != null)
                Application.Current.MainWindow.InputBindings.Add(FocusSearchBarBinding());
        }

        public ICommand FocusSearchBarCommand { get; }

        private InputBinding FocusSearchBarBinding()
        {
            var binding = new InputBinding(FocusSearchBarCommand, new KeyGesture(Key.F, ModifierKeys.Control));
            return binding;
        }

        private void FocusSearchBar(object obj) => SearchTextBox.Focus();
    }
}