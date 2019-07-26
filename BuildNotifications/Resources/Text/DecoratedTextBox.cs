using System.Windows;
using System.Windows.Controls;
using BuildNotifications.Resources.Icons;

namespace BuildNotifications.Resources.Text
{
    internal class DecoratedTextBox : TextBox
    {
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
            "Icon", typeof(IconType), typeof(DecoratedTextBox), new PropertyMetadata(default(IconType)));

        public IconType Icon
        {
            get => (IconType) GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(
            "Label", typeof(string), typeof(DecoratedTextBox), new PropertyMetadata(default(string)));

        public string Label
        {
            get => (string) GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }
    }
}