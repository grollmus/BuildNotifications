using System;
using BuildNotifications.Core.Pipeline.Tree;
using BuildNotifications.PluginInterfaces.Builds;

namespace BuildNotifications.ViewModel.Tree.Dummy
{
    internal class DefinitionGroupNodeDummy : BuildTreeNodeDummy, IDefinitionGroupNode
    {
        public DefinitionGroupNodeDummy(string definitionName)
        {
            Definition = new BuildDefinitionDummy
            {
                Name = definitionName
            };
        }

        public IBuildDefinition Definition { get; }

        private class BuildDefinitionDummy : IBuildDefinition
        {
            public bool Equals(IBuildDefinition other)
            {
                throw new NotImplementedException();
            }

            public string Id { get; set; }
            public string Name { get; set; }
        }
    }
}