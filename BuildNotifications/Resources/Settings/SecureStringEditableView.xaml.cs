using System.Windows;
using System.Windows.Controls;
using BuildNotifications.PluginInterfaces;
using ReflectSettings.EditableConfigs;

namespace BuildNotifications.Resources.Settings
{
    public partial class SecureStringEditableView
    {
        public SecureStringEditableView()
        {
            PasswordBox = new PasswordBox();
            PasswordBox.PasswordChanged += PasswordBoxOnPasswordChanged;
            InitializeComponent();
        }

        public EditableComplex<PasswordString> EditableSecureString
        {
            get => (EditableComplex<PasswordString>) GetValue(EditableSecureStringProperty);
            set => SetValue(EditableSecureStringProperty, value);
        }

        public PasswordBox PasswordBox { get; set; }

        private void EditableChanged()
        {
            if (!(EditableSecureString.Value is PasswordString existingValue))
                return;

            PasswordBox.Clear();
            PasswordBox.Password = existingValue.PlainText();
        }

        private void PasswordBoxOnPasswordChanged(object sender, RoutedEventArgs e)
        {
            if (EditableSecureString == null)
                return;

            EditableSecureString.Value = PasswordString.FromPlainText(PasswordBox.Password);
        }

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SecureStringEditableView view)
                view.EditableChanged();
        }

        public static readonly DependencyProperty EditableSecureStringProperty = DependencyProperty.Register(
            "EditableSecureString", typeof(EditableComplex<PasswordString>), typeof(SecureStringEditableView), new PropertyMetadata(default(EditableSecureString), PropertyChangedCallback));
    }
}