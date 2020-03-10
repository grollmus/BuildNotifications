using System;
using System.Runtime.InteropServices;
using System.Windows;

// ReSharper disable All
#pragma warning disable CA1060 // Move pinvokes to native methods class

namespace BuildNotifications.Resources.Window
{
   internal static class NativeUtils
   {
        static NativeUtils()
        {
            TPM_LEFTALIGN = 0;
            TPM_RETURNCMD = 256;
        }

        [DllImport("user32.dll", CharSet = CharSet.None, ExactSpelling = false)]
        internal static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = false, SetLastError = true)]
        internal static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll", CharSet = CharSet.None, ExactSpelling = false)]
        internal static extern IntPtr PostMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.None, ExactSpelling = false)]
        internal static extern int TrackPopupMenuEx(IntPtr hmenu, uint fuFlags, int x, int y, IntPtr hwnd, IntPtr lptpm);

        internal static readonly uint TPM_LEFTALIGN;

        internal static readonly uint TPM_RETURNCMD;
    }

    [Serializable]
    internal struct RECT
    {
        public int Left;

        public int Top;

        public int Right;

        public int Bottom;

        public int Height
        {
            get => Bottom - Top;
            set => Bottom = Top + value;
        }

        public Point Position => new Point(Left, Top);

        public Size Size => new Size(Width, Height);

        public int Width
        {
            get => Right - Left;
            set => Right = Left + value;
        }

        public RECT(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public RECT(Rect rect)
        {
            Left = (int) rect.Left;
            Top = (int) rect.Top;
            Right = (int) rect.Right;
            Bottom = (int) rect.Bottom;
        }

        public void Offset(int dx, int dy)
        {
            Left += dx;
            Right += dx;
            Top += dy;
            Bottom += dy;
        }

        public Int32Rect ToInt32Rect()
        {
            return new Int32Rect(Left, Top, Width, Height);
        }
    }
}