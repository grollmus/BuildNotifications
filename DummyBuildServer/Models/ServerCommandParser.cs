using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BuildNotifications.Plugin.DummyBuildServer;
using Newtonsoft.Json;

namespace DummyBuildServer.Models
{
    internal abstract class ServerCommandParser
    {
        protected ServerCommandParser(Server server)
        {
            Server = server;
        }

        public void ParseCommand(string command, Stream responseStream)
        {
            var commands = command.Split(Constants.Queries.Terminator);
            foreach (var commandString in commands)
            {
                var split = commandString.Split(' ', 2);
                var cmd = split[0];
                var args = split.Length > 1 ? split[1] : string.Empty;

                if (CommandMap.TryGetValue(cmd, out var action))
                    action(args, responseStream);
            }
        }

        protected async void WriteObject(Stream response, object data)
        {
            try
            {
                var settings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                };

                var json = JsonConvert.SerializeObject(data, Formatting.None, settings);
                var buffer = Encoding.ASCII.GetBytes(json);

                await response.WriteAsync(buffer, 0, buffer.Length);
                response.Flush();
            }
            catch (Exception)
            {
                Server.Stop();
            }
        }

        protected readonly Dictionary<string, Action<string, Stream>> CommandMap = new Dictionary<string, Action<string, Stream>>();
        protected readonly Server Server;
    }
}