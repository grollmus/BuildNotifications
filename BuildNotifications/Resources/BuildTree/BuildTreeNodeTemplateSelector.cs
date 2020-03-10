using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using BuildNotifications.ViewModel.Tree;

namespace BuildNotifications.Resources.BuildTree
{
    internal class BuildTreeNodeTemplateSelector : DataTemplateSelector
    {
        private BuildTreeNodeTemplateSelector(bool forLayout)
        {
            _forLayout = forLayout;
        }

        public static BuildTreeNodeTemplateSelector ForLevelLayout { get; } = new BuildTreeNodeTemplateSelector(true);

        public static BuildTreeNodeTemplateSelector ForNodeDisplay { get; } = new BuildTreeNodeTemplateSelector(false);

        public override DataTemplate? SelectTemplate(object item, DependencyObject container)
        {
            if (!(container is FrameworkElement element))
                return base.SelectTemplate(item, container);

            var buildNode = item as BuildTreeNodeViewModel;

            var template = _forLayout
                ? LayoutTemplate(buildNode, element)
                : DisplayTemplate(buildNode, element);

            return template ?? base.SelectTemplate(item, container);
        }

        private DataTemplate? DataTemplateByName(object groupNode, FrameworkElement element)
        {
            var type = groupNode.GetType();
            var fullName = type.Name;
            var withoutViewModel = fullName.Replace("ViewModel", "", StringComparison.InvariantCulture);
            var expectedKey = $"{withoutViewModel}Template";

            return TryFindResource(element, expectedKey);
        }

        private DataTemplate? DisplayTemplate(BuildTreeNodeViewModel? node, FrameworkElement element)
        {
            if (node == null)
                return null;

            return DataTemplateByName(node, element);
        }

        private DataTemplate? LayoutTemplate(BuildTreeNodeViewModel? buildNode, FrameworkElement element)
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
                default:
                    return TryFindResource(element, "FirstLevelTemplate");
                case 2:
                    return TryFindResource(element, "SecondLevelTemplate");
                case 3:
                    return TryFindResource(element, "ThirdLevelTemplate");
                case 4:
                    return TryFindResource(element, "FourthLevelTemplate");
            }
        }

        private DataTemplate? TryFindResource(FrameworkElement element, string key)
        {
            if (_cache.TryGetValue(key, out var existingTemplate))
                return existingTemplate;

            existingTemplate = element.TryFindResource(key) as DataTemplate;
            _cache.Add(key, existingTemplate);

            return existingTemplate;
        }

        private readonly Dictionary<string, DataTemplate?> _cache = new Dictionary<string, DataTemplate?>();

        private readonly bool _forLayout;
    }
}