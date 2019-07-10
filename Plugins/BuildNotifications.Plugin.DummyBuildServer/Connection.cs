using System.Diagnostics;
using System.IO.Pipes;
using System.Text;
using System.Threading.Tasks;

namespace BuildNotifications.Plugin.DummyBuildServer
{
    internal class Connection
    {
        public Connection(NamedPipeClientStream socket)
        {
            _socket = socket;
        }

        public Task<string> Query(string query)
        {
            var buffer = Encoding.ASCII.GetBytes(Prepare(query));
            Debug.WriteLine($"C Sending {buffer.Length} bytes: {query} ...");
            _socket.Write(buffer, 0, buffer.Length);
            Debug.WriteLine("C Sent");

            buffer = new byte[Constants.Connection.BufferSize];
            Debug.WriteLine("C Receiving response...");
            var received = _socket.Read(buffer, 0, buffer.Length);

            var response = Encoding.ASCII.GetString(buffer, 0, received);
            Debug.WriteLine($"C Received {received} bytes: {response}");

            return Task.FromResult(response);
        }

        private string Prepare(string query)
        {
            if (!query.EndsWith(Constants.Queries.Terminator))
                query += Constants.Queries.Terminator;

            return query;
        }

        private readonly NamedPipeClientStream _socket;
    }
}