using System.IO;
using System.Windows;
using Newtonsoft.Json;

namespace BuildNotifications.Services
{
    internal class WindowSettingsSerializer
    {
        public WindowSettingsSerializer(string fileName)
        {
            _fileName = fileName;
        }

        public WindowSettings Load()
        {
            if (!File.Exists(_fileName))
                return new WindowSettings();

            var json = File.ReadAllText(_fileName);
            return JsonConvert.DeserializeObject<WindowSettings>(json);
        }

        public void Save(Window window)
        {
            if (window.WindowState == WindowState.Minimized)
                return;

            var settings = new WindowSettings
            {
                Width = window.Width,
                Height = window.Height,
                Top = window.Top,
                Left = window.Left,
                State = window.WindowState
            };

            var json = JsonConvert.SerializeObject(settings);
            File.WriteAllText(_fileName, json);
        }

        private readonly string _fileName;
    }
}