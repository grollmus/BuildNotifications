using System.Windows;
using System.Windows.Controls;
using BuildNotifications.Resources.Icons;

namespace BuildNotifications.Resources.Settings
{
    internal class DecoratedComboBox : ComboBox
    {
        public DecoratedComboBox()
        {
            GotFocus += OnGotFocus;
        }

        public IconType Icon
        {
            get => (IconType) GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public string Label
        {
            get => (string) GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }

        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            if (!Equals(e.Source, this))
                return;
            IsDropDownOpen = true;
        }

        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
            "Icon", typeof(IconType), typeof(DecoratedComboBox), new PropertyMetadata(IconType.DownArrow));

        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(
            "Label", typeof(string), typeof(DecoratedComboBox), new PropertyMetadata(default(string)));
    }
}