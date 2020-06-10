using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
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
            catch
            {
                return new NativeMethods.WINDOWPLACEMENT();
            }
        }

        private readonly string _fileName;
    }
}