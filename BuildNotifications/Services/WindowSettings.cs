using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using Newtonsoft.Json;
using NLog.Fluent;

namespace BuildNotifications.Services
{
    internal class WindowSettings
    {
        public WindowSettings(string fileName)
        {
            _fileName = fileName;
        }

        public void ApplyTo(Window window)
        {
            var handle = new WindowInteropHelper(window).Handle;
            var placement = Load();

            if (ScreenSizeExceeded(placement))
            {
                Log.Debug().Message("Stored window position does not fit screen. Shrinking window.").Write();
                FitWindowOnScreen(ref placement, window);
            }

            if (!NativeMethods.SetWindowPlacement(handle, ref placement))
                Log.Warn().Message($"Failed to apply window placement to window. {NativeMethods.FormatLastWin32ErrorMessage()}").Write();
        }

        public void Save(Window window)
        {
            if (window.WindowState == WindowState.Minimized)
                return;

            var hwnd = new WindowInteropHelper(window).Handle;
            NativeMethods.GetWindowPlacement(hwnd, out var placement);

            var json = JsonConvert.SerializeObject(placement);
            File.WriteAllText(_fileName, json);
        }

        private static Screen FindScreen(in NativeMethods.WINDOWPLACEMENT placement)
        {
            var windowRect = new Rectangle(placement.normalPosition.Left, placement.normalPosition.Top, placement.normalPosition.Width, placement.normalPosition.Height);
            return Screen.FromRectangle(windowRect);
        }

        private static void FitWindowOnScreen(ref NativeMethods.WINDOWPLACEMENT placement, Window window)
        {
            var screen = FindScreen(placement);

            var maxAllowedWidth = Math.Min(screen.WorkingArea.Width, placement.normalPosition.Width);
            var targetWidth = Math.Max(maxAllowedWidth, window.MinWidth);
            var x = screen.WorkingArea.Width / 2.0 - targetWidth / 2 + screen.Bounds.Left;

            var maxAllowedHeight = Math.Min(screen.WorkingArea.Height, placement.normalPosition.Height);
            var targetHeight = Math.Max(maxAllowedHeight, window.MinHeight);
            var y = screen.WorkingArea.Height / 2.0 - targetHeight / 2 + screen.Bounds.Top;

            placement.normalPosition.Top = (int) y;
            placement.normalPosition.Left = (int) x;
            placement.normalPosition.Height = (int) targetHeight;
            placement.normalPosition.Width = (int) targetWidth;
        }

        private NativeMethods.WINDOWPLACEMENT Load()
        {
            if (!File.Exists(_fileName))
                return new NativeMethods.WINDOWPLACEMENT();

            try
            {
                var json = File.ReadAllText(_fileName);
                var placement = JsonConvert.DeserializeObject<NativeMethods.WINDOWPLACEMENT>(json);
                placement.length = Marshal.SizeOf<NativeMethods.WINDOWPLACEMENT>();
                placement.flags = 0;
                placement.showCmd = placement.showCmd == NativeMethods.SW_SHOWMINIMIZED ? NativeMethods.SW_SHOWNORMAL : placement.showCmd;
                return placement;
            }
            catch (Exception ex)
            {
                Log.Warn().Exception(ex).Message("Failed to load window placement from file").Write();
                return new NativeMethods.WINDOWPLACEMENT();
            }
        }

        private static bool ScreenSizeExceeded(in NativeMethods.WINDOWPLACEMENT placement)
        {
            var screen = FindScreen(placement);
            return !screen.WorkingArea.Contains(placement.normalPosition.Right, placement.normalPosition.Bottom);
        }

        private readonly string _fileName;
    }
}