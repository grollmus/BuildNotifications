using System.Windows;
using System.Windows.Controls;
using BuildNotifications.PluginInterfaces;
using ReflectSettings.EditableConfigs;

namespace BuildNotifications.Resources.Settings
{
    public partial class SecureStringEditableView
    {
        public PasswordBox PasswordBox { get; set; }

        public static readonly DependencyProperty EditableSecureStringProperty = DependencyProperty.Register(
            "EditableSecureString", typeof(EditableComplex<PasswordString>), typeof(SecureStringEditableView), new PropertyMetadata(default(EditableSecureString), PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SecureStringEditableView view)
                view.EditableChanged();
        }

        private void EditableChanged()
        {
            if (!(EditableSecureString.Value is PasswordString existingValue))
                return;

            PasswordBox.Clear();
            PasswordBox.Password = existingValue.PlainText();
        }

        public EditableComplex<PasswordString> EditableSecureString
        {
            get => (EditableComplex<PasswordString>) GetValue(EditableSecureStringProperty);
            set => SetValue(EditableSecureStringProperty, value);
        }

        public SecureStringEditableView()
        {
            PasswordBox = new PasswordBox();
            PasswordBox.PasswordChanged += PasswordBoxOnPasswordChanged;
            InitializeComponent();
        }

        private void PasswordBoxOnPasswordChanged(object sender, RoutedEventArgs e)
        {
            if (EditableSecureString == null)
                return;

            EditableSecureString.Value = PasswordString.FromPlainText(PasswordBox.Password);
        }
    }
}