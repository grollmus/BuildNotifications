using System;
using System.Diagnostics;
using System.IO.Pipes;
using System.Text;
using System.Threading.Tasks;

namespace BuildNotifications.Plugin.DummyBuildServer
{
    internal class Connection
    {
        public Connection(int port)
        {
            _port = port;
        }

        public async Task<string> Query(string query)
        {
            await using var socket = await Connect();

            var buffer = Encoding.ASCII.GetBytes(Prepare(query));
            Debug.WriteLine($"C Sending {buffer.Length} bytes: {query} ...");
            socket.Write(buffer, 0, buffer.Length);
            Debug.WriteLine("C Sent");

            buffer = new byte[Constants.Connection.BufferSize];
            Debug.WriteLine("C Receiving response...");

            var responseBuilder = new StringBuilder();
            var receivedBytesSum = 0;
            int receivedBytesLastRead;

            do
            {
                receivedBytesLastRead = socket.Read(buffer, 0, buffer.Length);
                receivedBytesSum += receivedBytesLastRead;
                var response = Encoding.ASCII.GetString(buffer, 0, receivedBytesLastRead);
                responseBuilder.Append(response);
            } while (receivedBytesLastRead >= buffer.Length);

            Debug.WriteLine($"C Received {receivedBytesSum} bytes: {responseBuilder}");

            return responseBuilder.ToString();
        }

        internal async Task<NamedPipeClientStream> Connect()
        {
            var socket = new NamedPipeClientStream(".", $"BuildNotifications.DummyBuildServer.{_port}", PipeDirection.InOut);

            await socket.ConnectAsync((int) TimeSpan.FromSeconds(5).TotalMilliseconds);

            return socket;
        }

        private string Prepare(string query)
        {
            if (!query.EndsWith(Constants.Queries.Terminator))
                query += Constants.Queries.Terminator;

            return query;
        }

        private readonly int _port;
    }
}