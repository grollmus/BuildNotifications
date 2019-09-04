﻿using BuildNotifications.PluginInterfaces.Builds;

namespace BuildNotifications.Core.Tests
{
    internal class MockBuildDefinition : IBuildDefinition
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
            return mock?.Id.Equals(Id) == true;
        }

        public string Id { get; set; }
        public string Name { get; set; }
    }
}