using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.IO.Pipes;
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

        public List<Build> Builds { get; } = new List<Build>();

        public ServerConfig Config { get; }

        public bool IsRunning { get; private set; }

        public void Start(int port)
        {
            _port = port;
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
            var pipeServer = new NamedPipeServerStream($"BuildNotifications.DummyBuildServer.{_port}", PipeDirection.InOut, 1);
            pipeServer.WaitForConnection();

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
        private Thread? _networkThread;
        private MemoryMappedFile _memoryFile;
        private int _port;
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

        private void WriteObject(Stream response, object data)
        {
            var settings = new JsonSerializerSettings();
            settings.TypeNameHandling = TypeNameHandling.Auto;

            var json = JsonConvert.SerializeObject(data, Formatting.None, settings);
            var buffer = Encoding.ASCII.GetBytes(json);
            response.Write(buffer, 0, buffer.Length);
            response.Flush();
        }

        private readonly Dictionary<string, Action<string, Stream>> _commandMap = new Dictionary<string, Action<string, Stream>>();
        private readonly Server _server;
    }
}