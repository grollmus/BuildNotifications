using BuildNotifications.Core.Pipeline.Tree.Arrangement;

namespace BuildNotifications.Resources.Icons
{
    internal static class GroupDefinitionExtensions
    {
        public static IconType ToIconType(this GroupDefinition groupDefinition)
        {
            switch (groupDefinition)
            {
                case GroupDefinition.Branch:
                    return IconType.Branch;
                case GroupDefinition.BuildDefinition:
                    return IconType.Definition;
                case GroupDefinition.Source:
                    return IconType.Connection;
                case GroupDefinition.Status:
                    return IconType.Status;
                case GroupDefinition.None:
                default:
                    return IconType.None;
            }
        }
    }
}