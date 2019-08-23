using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BuildNotifications.Plugin.DummyBuildServer;
using Newtonsoft.Json;

namespace DummyBuildServer.Models
{
    internal class Server
    {
        public Server(ServerConfig config)
        {
            Config = config;
            _parser = new ServerCommandParser(this);
            _buildNotificationsProcessHook = new BuildNotificationsProcessHook();
            _buildNotificationsProcessHook.OnProcessExited += OnBuildNotificationsProcessTerminated;
        }

        public List<Build> Builds { get; } = new List<Build>();

        public ServerConfig Config { get; }

        public bool IsRunning { get; private set; }

        public void Start(int port)
        {
            _port = port;
            _cancelToken = new CancellationTokenSource();
            _networkThread = new Thread(Run);
            IsRunning = true;
            _networkThread.Start();

            _memoryFile = MemoryMappedFile.CreateNew("BuildNotifications.DummyBuildServer", 1, MemoryMappedFileAccess.ReadWrite);
        }

        public void Stop()
        {
            IsRunning = false;
            _cancelToken?.Cancel();
            _networkThread?.Join();
            _memoryFile.Dispose();
        }

        private async void OnBuildNotificationsProcessTerminated(object sender, EventArgs e)
        {
            Debug.WriteLine("BuildNotifications.exe terminated. Auto restarting server...");
            Stop();
            // waiting on windows to actually close the pipes
            await Task.Delay(100);
            Start(_port);
        }

        private async void Run()
        {
            var pipeServer = new NamedPipeServerStream($"BuildNotifications.DummyBuildServer.{_port}", PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);

            try
            {
                await pipeServer.WaitForConnectionAsync(_cancelToken.Token);
            }
            catch (Exception)
            {
                pipeServer.Close();
                return;
            }

            _buildNotificationsProcessHook.SearchForProcess();

            pipeServer.ReadMode = PipeTransmissionMode.Byte;

            var buffer = new byte[Constants.Connection.BufferSize];

            while (IsRunning)
            {
                Debug.WriteLine("S Receiving data...");
                var bufferLength = pipeServer.Read(buffer, 0, Constants.Connection.BufferSize);
                Debug.WriteLine($"S Received {bufferLength} bytes");
                var command = Encoding.ASCII.GetString(buffer, 0, bufferLength);

                _parser.ParseCommand(command, pipeServer);

                Thread.Sleep(10);
            }

            pipeServer.Close();
        }

        private readonly ServerCommandParser _parser;
        private readonly BuildNotificationsProcessHook _buildNotificationsProcessHook;

        private CancellationTokenSource _cancelToken;
        private Thread? _networkThread;
        private MemoryMappedFile _memoryFile;
        private int _port;
    }

    internal class BuildNotificationsProcessHook
    {
        public event EventHandler OnProcessExited;

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
            await Task.Run(() => _process.WaitForExit());

            _process = null;
            OnProcessExited?.Invoke(this, EventArgs.Empty);
        }

        private Process _process;
    }

    internal class ServerCommandParser
    {
        public ServerCommandParser(Server server)
        {
            _server = server;

            _commandMap[Constants.Queries.Branches] = ListBranches;
            _commandMap[Constants.Queries.Definitions] = ListDefinitions;
            _commandMap[Constants.Queries.Builds] = ListBuilds;
        }

        public void ParseCommand(string command, Stream responseStream)
        {
            var commands = command.Split(Constants.Queries.Terminator);
            foreach (var commandString in commands)
            {
                var split = commandString.Split(' ', 2);
                var cmd = split[0];
                var args = split.Length > 1 ? split[1] : string.Empty;

                if (_commandMap.TryGetValue(cmd, out var action))
                    action(args, responseStream);
            }
        }

        private void ListBranches(string args, Stream response)
        {
            WriteObject(response, _server.Config.Branches);
        }

        private void ListBuilds(string command, Stream responseStream)
        {
            WriteObject(responseStream, _server.Builds);
        }

        private void ListDefinitions(string command, Stream response)
        {
            WriteObject(response, _server.Config.BuildDefinitions);
        }

        private async void WriteObject(Stream response, object data)
        {
            try
            {
                var settings = new JsonSerializerSettings();
                settings.TypeNameHandling = TypeNameHandling.Auto;

                var json = JsonConvert.SerializeObject(data, Formatting.None, settings);
                var buffer = Encoding.ASCII.GetBytes(json);
                
                await response.WriteAsync(buffer, 0, buffer.Length);
                response.Flush();
            }
            catch (Exception)
            {
                _server.Stop();
            }
        }

        private readonly Dictionary<string, Action<string, Stream>> _commandMap = new Dictionary<string, Action<string, Stream>>();
        private readonly Server _server;
    }
}