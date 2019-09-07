using System.Windows;
using System.Windows.Media;

namespace BuildNotifications.Resources.Icons
{
    /// <summary>
    /// Interaction logic for IconBatch.xaml
    /// </summary>
    public partial class IconBatch
    {
        public IconBatch()
        {
            InitializeComponent();
        }

        public new Brush Foreground
        {
            get => (Brush) GetValue(ForegroundProperty);
            set => SetValue(ForegroundProperty, value);
        }

        public IconType Type
        {
            get => (IconType) GetValue(TypeProperty);
            set => SetValue(TypeProperty, value);
        }

        public new static readonly DependencyProperty ForegroundProperty = DependencyProperty.Register(
            "Foreground", typeof(Brush), typeof(IconBatch), new PropertyMetadata(default(Brush)));

        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register(
            "Type", typeof(IconType), typeof(IconBatch), new PropertyMetadata(default(IconType)));
    }
}