using System;
using System.Diagnostics;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using BuildNotifications.Plugin.DummyBuildServer;

namespace DummyBuildServer.Models
{
    internal class ServerThread : IDisposable
    {
        public ServerThread(int port, ServerCommandParser parser, CancellationToken cancellationToken)
        {
            _port = port;
            _parser = parser;
            _cancellationToken = cancellationToken;
        }

        public void Start()
        {
            _isRunning = true;
            _thread = new Thread(Run);
            _thread.Start();
        }

        private async void Run()
        {
            var pipeServer = new NamedPipeServerStream($"BuildNotifications.DummyBuildServer.{_port}", PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);

            try
            {
                await pipeServer.WaitForConnectionAsync(_cancellationToken);
            }
            catch (Exception)
            {
                pipeServer.Close();
                return;
            }

            pipeServer.ReadMode = PipeTransmissionMode.Byte;

            var buffer = new byte[Constants.Connection.BufferSize];

            while (_isRunning)
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

        public void Dispose()
        {
            _isRunning = false;
            _thread?.Join();
        }

        private readonly int _port;
        private readonly ServerCommandParser _parser;
        private readonly CancellationToken _cancellationToken;
        private Thread? _thread;
        private volatile bool _isRunning;
    }
}