using System;
using System.Windows.Media;

// ReSharper disable BuiltInTypeReferenceStyle

namespace BuildNotifications.ViewModel.Utils
{
    public static class ColorConvertExtension
    {
        public static Color ToWindowsColor(this UInt32 color) => Color.FromArgb((byte) (color >> 24 & 0xff), (byte) (color >> 16 & 0xff), (byte) (color >> 8 & 0xff), (byte) (color & 0xff));

        public static UInt32 ToUintColor(this Color color) => (UInt32) color.A << 24 | (UInt32) color.R << 16 | (UInt32) color.G << 8 | color.B;
    }
}