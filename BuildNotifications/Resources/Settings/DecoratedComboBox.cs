using System.Windows;
using System.Windows.Controls;
using BuildNotifications.Resources.Icons;

namespace BuildNotifications.Resources.Settings
{
    internal class DecoratedComboBox : ComboBox
    {
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
            "Icon", typeof(IconType), typeof(DecoratedComboBox), new PropertyMetadata(default(IconType)));

        public IconType Icon
        {
            get => (IconType) GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(
            "Label", typeof(string), typeof(DecoratedComboBox), new PropertyMetadata(default(string)));

        public string Label
        {
            get => (string) GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }

        public DecoratedComboBox()
        {
            GotFocus += OnGotFocus;
        }

        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            if (!Equals(e.Source, this))
                return;
            IsDropDownOpen = true;
        }
    }
}
