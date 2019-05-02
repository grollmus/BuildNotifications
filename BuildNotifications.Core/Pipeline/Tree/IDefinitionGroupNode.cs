using BuildNotifications.PluginInterfaces.Builds;

namespace BuildNotifications.Core.Pipeline.Tree
{
    internal interface IDefinitionGroupNode : IGroupNode
    {
        IBuildDefinition Definition { get; }
    }
}