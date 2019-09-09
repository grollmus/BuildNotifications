using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows;
using System.Windows.Controls;
using ReflectSettings.EditableConfigs;

namespace BuildNotifications.Resources.Settings
{
    public partial class SecureStringEditableView
    {
        public PasswordBox PasswordBox { get; set; }

        public static readonly DependencyProperty EditableSecureStringProperty = DependencyProperty.Register(
            "EditableSecureString", typeof(EditableSecureString), typeof(SecureStringEditableView), new PropertyMetadata(default(EditableSecureString), PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SecureStringEditableView view)
                view.EditableChanged();
        }

        private void EditableChanged()
        {
            if (!(EditableSecureString.Value is SecureString existingValue))
                return;

            PasswordBox.Clear();
            var valuePtr = IntPtr.Zero;
            try
            {
                valuePtr = Marshal.SecureStringToGlobalAllocUnicode(existingValue);
                for (var i = 0; i < existingValue.Length; i++)
                {
                    var unicodeChar = Marshal.ReadInt16(valuePtr, i * 2);
                    PasswordBox.Password += (char) unicodeChar;
                }
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
            }
        }

        public EditableSecureString EditableSecureString
        {
            get => (EditableSecureString) GetValue(EditableSecureStringProperty);
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

            EditableSecureString.Value = PasswordBox.SecurePassword;
        }
    }
}