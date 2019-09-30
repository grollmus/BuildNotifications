using System;
using BuildNotifications.Core.Config;
using BuildNotifications.Core.Plugin;
using JetBrains.Annotations;
using ReflectSettings.Attributes;

namespace BuildNotifications.ViewModel.Settings
{
    public class ConnectionDataViewModel
    {
        public ConnectionDataViewModel(ConnectionData connection, IPluginRepository pluginRepository)
        {
            Connection = connection;
            TestConnectionViewModel = new TestConnectionViewModel(this);
            TestConnectionViewModel.TestFinished += (sender, args) => TestFinished?.Invoke(this, EventArgs.Empty);
            PluginRepository = pluginRepository;
        }

        public ConnectionDataViewModel()
        {
            TestConnectionViewModel = new TestConnectionViewModel(this);
            TestConnectionViewModel.TestFinished += (sender, args) => TestFinished?.Invoke(this, EventArgs.Empty);
        }

        [IsDisplayName]
        [UsedImplicitly]
        public string Name
        {
            get => Connection?.Name ?? "";
            set
            {
                if (Connection == null)
                    return;
                Connection.Name = value;
            }
        }

        public ConnectionData? Connection { get; set; }

        [IgnoredForConfig]
        public IPluginRepository? PluginRepository { get; set; }

        public TestConnectionViewModel TestConnectionViewModel { get; set; }

        public event EventHandler? TestFinished;
    }
}