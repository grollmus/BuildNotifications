using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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
        }

        public ServerConfig Config { get; }

        public bool IsRunning { get; private set; }

        public void Start(int port)
        {
            _listener = new TcpListener(IPAddress.Any, port);
            _networkThread = new Thread(Run);
            IsRunning = true;
            _networkThread.Start();

            _memoryFile = MemoryMappedFile.CreateNew("BuildNotifications.DummyBuildServer", 1, MemoryMappedFileAccess.ReadWrite);
        }

        public void Stop()
        {
            IsRunning = false;
            _networkThread?.Join();

            _memoryFile.Dispose();
        }

        private void Run()
        {
            _listener!.Start();
            var client = _listener.AcceptTcpClient();
            var stream = client.GetStream();

            const int bufferSize = 8192;
            var buffer = new byte[bufferSize];

            while (IsRunning)
            {
                if (stream.DataAvailable)
                {
                    var bufferLength = stream.Read(buffer, 0, bufferSize);
                    var command = Encoding.ASCII.GetString(buffer, 0, bufferLength);

                    _parser.ParseCommand(command, stream);
                }

                Thread.Sleep(10);
            }

            client.Dispose();
            _listener?.Stop();
        }

        private readonly ServerCommandParser _parser;

        private TcpListener? _listener;
        private Thread? _networkThread;
        private MemoryMappedFile _memoryFile;
    }

    internal class ServerCommandParser
    {
        public ServerCommandParser(Server server)
        {
            _server = server;

            _commandMap[Constants.Queries.Branches] = ListBranches;
        }

        public void ParseCommand(string command, Stream stream)
        {
            var split = command.Split(' ', 2);
            var cmd = split[0];
            var args = split.Length > 1 ? split[1] : string.Empty;

            if (_commandMap.TryGetValue(cmd, out var action))
            {
                action(args, stream);
            }
        }

        private void ListBranches(string args, Stream response)
        {
            var json = JsonConvert.SerializeObject(_server.Config.Branches);
            var buffer = Encoding.ASCII.GetBytes(json);
            response.Write(buffer, 0, buffer.Length);
        }

        private readonly Dictionary<string, Action<string, Stream>> _commandMap = new Dictionary<string, Action<string, Stream>>();
        private readonly Server _server;
    }
}