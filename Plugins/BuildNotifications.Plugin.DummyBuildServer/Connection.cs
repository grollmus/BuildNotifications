using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BuildNotifications.Plugin.DummyBuildServer
{
    internal class Connection
    {
        public Connection(TcpClient client)
        {
            _client = client;
        }

        public async Task<string> Query(string query)
        {
            var stream = _client.GetStream();

            var buffer = Encoding.ASCII.GetBytes(query);
            await stream.WriteAsync(buffer, 0, buffer.Length);

            const int bufferSize = 8192;
            buffer = new byte[bufferSize];
            var received = await stream.ReadAsync(buffer, 0, bufferSize);

            var response = Encoding.ASCII.GetString(buffer, 0, received);
            return response;
        }

        private readonly TcpClient _client;
    }
}