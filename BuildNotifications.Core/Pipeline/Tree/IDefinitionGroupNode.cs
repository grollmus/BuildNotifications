using BuildNotifications.PluginInterfaces.Builds;

namespace BuildNotifications.Core.Pipeline.Tree
{
    public interface IDefinitionGroupNode : IGroupNode
    {
        IBuildDefinition Definition { get; }
    }
}