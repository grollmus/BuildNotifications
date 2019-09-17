using System.Windows;
using System.Windows.Media;

namespace BuildNotifications.Resources.Icons
{
    public partial class SimpleIconBatch
    {
        public SimpleIconBatch()
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
            "Foreground", typeof(Brush), typeof(SimpleIconBatch), new PropertyMetadata(default(Brush)));

        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register(
            "Type", typeof(IconType), typeof(SimpleIconBatch), new PropertyMetadata(default(IconType)));
    }
}