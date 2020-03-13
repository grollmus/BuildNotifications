using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DummyBuildServer.Models
{
    internal class BuildNotificationsProcessHook
    {
        public event EventHandler? OnProcessExited;

        public void SearchForProcess()
        {
            if (_process != null)
                return;

            var process = Process.GetProcessesByName("BuildNotifications").FirstOrDefault();
            if (process != null)
            {
                Debug.WriteLine("Found BuildNotifications.exe. Waiting for termination for auto restart.");
                _process = process;
                WaitForProcess();
            }
        }

        private async void WaitForProcess()
        {
            if (_process != null)
                await Task.Run(() => _process.WaitForExit());

            _process = null;
            OnProcessExited?.Invoke(this, EventArgs.Empty);
        }

        private Process? _process;
    }
}