using System;
using System.Diagnostics;
using System.IO.Pipes;
using System.Text;
using System.Threading.Tasks;

namespace BuildNotifications.Plugin.DummyBuildServer
{
    internal class Connection : IDisposable
    {
        public Connection(NamedPipeClientStream socket)
        {
            _socket = socket;
        }

        public async Task<string> Query(string query)
        {
            await Connect();

            var buffer = Encoding.ASCII.GetBytes(Prepare(query));
            Debug.WriteLine($"C Sending {buffer.Length} bytes: {query} ...");
            _socket.Write(buffer, 0, buffer.Length);
            Debug.WriteLine("C Sent");

            buffer = new byte[Constants.Connection.BufferSize];
            Debug.WriteLine("C Receiving response...");
            var received = _socket.Read(buffer, 0, buffer.Length);

            var response = Encoding.ASCII.GetString(buffer, 0, received);
            Debug.WriteLine($"C Received {received} bytes: {response}");

            return response;
        }

        internal async Task Connect()
        {
            if (!_socket.IsConnected)
                await _socket.ConnectAsync((int) TimeSpan.FromSeconds(5).TotalMilliseconds);
        }

        private string Prepare(string query)
        {
            if (!query.EndsWith(Constants.Queries.Terminator))
                query += Constants.Queries.Terminator;

            return query;
        }

        public void Dispose()
        {
            _socket?.Dispose();
        }

        private readonly NamedPipeClientStream _socket;
    }
}