using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BuildNotifications.Resources.Icons
{
    /// <summary>
    /// Interaction logic for IconBatch.xaml
    /// </summary>
    public partial class IconBatch : UserControl
    {
        public static readonly DependencyProperty ForegroundProperty = DependencyProperty.Register(
            "Foreground", typeof(Brush), typeof(IconBatch), new PropertyMetadata(default(Brush)));

        public Brush Foreground
        {
            get => (Brush) GetValue(ForegroundProperty);
            set => SetValue(ForegroundProperty, value);
        }

        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register(
            "Type", typeof(IconType), typeof(IconBatch), new PropertyMetadata(default(IconType)));

        public IconType Type
        {
            get => (IconType) GetValue(TypeProperty);
            set => SetValue(TypeProperty, value);
        }

        public IconBatch()
        {
            InitializeComponent();
        }
    }

    public enum IconType
    {
        Connection,
        Branch,
        Definition,
        BuildNotification,
        Close,
        Maximize,
        Minimize,
        Restore,
    }
}
