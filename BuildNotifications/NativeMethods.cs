using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

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
        public static extern bool GetWindowPlacement(IntPtr hWnd, out WindowPlacement lpWndPl);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetWindowPlacement(IntPtr hWnd, [In] ref WindowPlacement lpWndPl);

        public const int SwShowMinimized = 2;
        public const int SwShowNormal = 1;

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;

            [UsedImplicitly]
            public Rect(int left, int top, int right, int bottom)
            {
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }
        }

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct Point
        {
            public int X;
            public int Y;

            [UsedImplicitly]
            public Point(int x, int y)
            {
                X = x;
                Y = y;
            }
        }

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct WindowPlacement
        {
            public int length;
            public int flags;
            public int showCmd;
            [UsedImplicitly] public Point minPosition;
            [UsedImplicitly] public Point maxPosition;
            [UsedImplicitly] public Rect normalPosition;
        }
    }
}