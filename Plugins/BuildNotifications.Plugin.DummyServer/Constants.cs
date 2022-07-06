namespace BuildNotifications.Plugin.DummyServer;

public static class Constants
{
    public static class Connection
    {
        public const int BufferSize = 65536;
        public const string Port = "port";
    }

    public static class Queries
    {
        public const string Branches = "branches";
        public const string Builds = "builds";
        public const string Definitions = "definitions";
        public const string Terminator = ";";
    }
}