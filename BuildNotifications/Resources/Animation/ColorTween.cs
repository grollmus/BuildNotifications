using System.Windows.Media;

namespace BuildNotifications.Resources.Animation
{
    internal static class ColorTween
    {
        public static Color ColorProgressFunction(Color startValue, Color endValue, double position)
        {
            return Color.FromArgb(
                (byte) (startValue.A + (endValue.A - startValue.A) * position),
                (byte) (startValue.R + (endValue.R - startValue.R) * position),
                (byte) (startValue.G + (endValue.G - startValue.G) * position),
                (byte) (startValue.B + (endValue.B - startValue.B) * position)
            );
        }
    }
}