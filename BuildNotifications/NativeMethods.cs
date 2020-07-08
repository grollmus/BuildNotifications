using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using JetBrains.Annotations;

// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

namespace BuildNotifications
{
    internal static class NativeMethods
    {
        public static string FormatLastWin32ErrorMessage()
        {
            var errorCode = Marshal.GetLastWin32Error();
            var errorMessage = new Win32Exception(errorCode).Message;

            return $"{errorMessage} ({errorCode})";
        }

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetWindowPlacement(IntPtr hWnd, out WINDOWPLACEMENT lpWndPl);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetWindowPlacement(IntPtr hWnd, [In] ref WINDOWPLACEMENT lpWndPl);

        [DllImport("user32.dll", CharSet = CharSet.None, ExactSpelling = false)]
        internal static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = false, SetLastError = true)]
        internal static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll", CharSet = CharSet.None, ExactSpelling = false)]
        internal static extern IntPtr PostMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.None, ExactSpelling = false)]
        internal static extern int TrackPopupMenuEx(IntPtr hmenu, uint fuFlags, int x, int y, IntPtr hwnd, IntPtr lptpm);

        public const int SW_SHOWMINIMIZED = 2;
        public const int SW_SHOWNORMAL = 1;

        internal const uint TPM_LEFTALIGN = 0;

        internal const uint TPM_RETURNCMD = 256;

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
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

            public Int32Rect ToInt32Rect() => new Int32Rect(Left, Top, Width, Height);
        }

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            [UsedImplicitly]
            public POINT(int x, int y)
            {
                X = x;
                Y = y;
            }
        }

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public int showCmd;
            [UsedImplicitly] public POINT minPosition;
            [UsedImplicitly] public POINT maxPosition;
            [UsedImplicitly] public RECT normalPosition;
        }
    }
}