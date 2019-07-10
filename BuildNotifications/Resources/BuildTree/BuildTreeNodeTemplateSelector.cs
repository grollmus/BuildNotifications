using System;
using System.Windows;
using System.Windows.Controls;
using BuildNotifications.ViewModel.Tree;

namespace BuildNotifications.Resources.BuildTree
{
    internal class BuildTreeNodeTemplateSelector : DataTemplateSelector
    {
        private readonly bool _forLayout;

        public static BuildTreeNodeTemplateSelector ForNodeDisplay { get; } = new BuildTreeNodeTemplateSelector(false);
        public static BuildTreeNodeTemplateSelector ForLevelLayout { get; } = new BuildTreeNodeTemplateSelector(true);

        private BuildTreeNodeTemplateSelector(bool forLayout)
        {
            _forLayout = forLayout;
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var element = container as FrameworkElement;
            DataTemplate template;
            var buildNode = item as BuildTreeNodeViewModel;

            template =
                _forLayout
                    ? LayoutTemplate(buildNode, element)
                    : DisplayTemplate(buildNode, element);

            if (template == null)
                return base.SelectTemplate(item, container);
            else
                return template;
        }

        private DataTemplate LayoutTemplate(BuildTreeNodeViewModel buildNode, FrameworkElement element)
        {
            if (buildNode == null)
                return null;

            // display groups with the least possible grouping. Therefore start with the smallest grouping (level 4) and extend to the biggest (level 1)
            // in other words, for 3 nested groups, the layout for a fourth group level is not needed. However this fourth group WOULD not be level 4, it would be level 1.
            // Therefore with 3 groups, display the DataTemplates 4, 3, 2.
            var distanceToDeepestLevel = buildNode.MaxTreeDepth - buildNode.CurrentTreeLevelDepth;
            var levelToDisplay = Math.Max(1, 4 - distanceToDeepestLevel); // 4 is the maximum of supported depths, beyond that display every group the same as level 1
            switch (levelToDisplay)
            {
                case 1:
                default:
                    return element.TryFindResource("FirstLevelTemplate") as DataTemplate;
                case 2:
                    return element.TryFindResource("SecondLevelTemplate") as DataTemplate;
                case 3:
                    return element.TryFindResource("ThirdLevelTemplate") as DataTemplate;
                case 4:
                    return element.TryFindResource("FourthLevelTemplate") as DataTemplate;
            }
        }

        private DataTemplate DisplayTemplate(BuildTreeNodeViewModel node, FrameworkElement element)
        {
            return DataTemplateByName(node, element);
        }

        private static DataTemplate DataTemplateByName(object groupNode, FrameworkElement element)
        {
            var type = groupNode.GetType();
            var fullName = type.Name;
            var withoutViewModel = fullName.Replace("ViewModel", "");
            var expectedKey = $"{withoutViewModel}Template";

            return element.TryFindResource(expectedKey) as DataTemplate;
        }
    }
}