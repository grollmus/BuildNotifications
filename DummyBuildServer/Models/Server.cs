using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.MemoryMappedFiles;
using System.Threading;
using System.Threading.Tasks;
using BuildNotifications.Plugin.DummyBuildServer;

namespace DummyBuildServer.Models
{
    internal class Server
    {
        public Server(ServerConfig config)
        {
            Config = config;
            _buildCommandParser = new BuildCommandParser(this);
            _sourceControlCommandParser = new SourceControlCommandParser(this);
            _buildNotificationsProcessHook = new BuildNotificationsProcessHook();
            _buildNotificationsProcessHook.OnProcessExited += OnBuildNotificationsProcessTerminated;
            _cancelToken = new CancellationTokenSource();
        }

        public List<Build> Builds { get; } = new List<Build>();

        public ServerConfig Config { get; }

        public bool IsRunning { get; private set; }

        public void Start(int buildPort, int sourceControlPort)
        {
            IsRunning = true;
            _buildPort = buildPort;
            _sourceControlPort = sourceControlPort;
            _cancelToken = new CancellationTokenSource();
            _networkThreadBuild = new ServerThread(_buildPort, _buildCommandParser, _cancelToken.Token);
            _networkThreadSourceControl = new ServerThread(_sourceControlPort, _sourceControlCommandParser, _cancelToken.Token);
            _networkThreadBuild.Start();
            _networkThreadSourceControl.Start();

            _memoryFile = MemoryMappedFile.CreateNew("BuildNotifications.DummyBuildServer", 1, MemoryMappedFileAccess.ReadWrite);

            _buildNotificationsProcessHook.SearchForProcess();
        }

        public void Stop()
        {
            IsRunning = false;
            _cancelToken.Cancel();
            _networkThreadBuild?.Dispose();
            _networkThreadSourceControl?.Dispose();
            _memoryFile?.Dispose();
        }

        private async void OnBuildNotificationsProcessTerminated(object? sender, EventArgs e)
        {
            Debug.WriteLine("BuildNotifications.exe terminated. Auto restarting server...");
            Stop();
            // waiting on windows to actually close the pipes
            await Task.Delay(100);
            Start(_buildPort, _sourceControlPort);
        }

        private readonly ServerCommandParser _buildCommandParser;
        private readonly BuildNotificationsProcessHook _buildNotificationsProcessHook;
        private readonly SourceControlCommandParser _sourceControlCommandParser;
        private CancellationTokenSource _cancelToken;
        private ServerThread? _networkThreadBuild;
        private ServerThread? _networkThreadSourceControl;
        private MemoryMappedFile? _memoryFile;
        private int _buildPort;
        private int _sourceControlPort;
    }
}