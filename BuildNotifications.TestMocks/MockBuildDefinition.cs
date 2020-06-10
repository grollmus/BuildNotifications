using System;
using BuildNotifications.PluginInterfaces.Builds;

namespace BuildNotifications.TestMocks
{
    public class MockBuildDefinition : IBuildDefinition
    {
        public MockBuildDefinition()
        {
            Id = string.Empty;
            Name = string.Empty;
        }

        public MockBuildDefinition(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public bool Equals(IBuildDefinition other)
        {
            var mock = other as MockBuildDefinition;
            return mock?.Id.Equals(Id, StringComparison.InvariantCulture) == true;
        }

        public string Id { get; set; }
        public string Name { get; set; }
    }
}