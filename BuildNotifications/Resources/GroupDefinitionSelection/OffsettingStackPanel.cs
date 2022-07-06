using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BuildNotifications.ViewModel.Utils;

namespace BuildNotifications.Resources.GroupDefinitionSelection;

internal class OffsettingStackPanel : StackPanel
{
    public OffsettingStackPanel()
    {
        Orientation = Orientation.Vertical;
    }

    public double Offset { get; set; }

    protected override Size ArrangeOverride(Size arrangeSize)
    {
        var children = InternalChildren;
        var rcChild = new Rect(arrangeSize);
        var previousChildSize = 0.0;
        var previousXOffset = -MaxOffset();

        for (int i = 0, count = children.Count; i < count; ++i)
        {
            var child = children[i];
            if (child == null || child.Visibility == Visibility.Collapsed)
                continue;

            rcChild.Y += previousChildSize;
            rcChild.X = previousXOffset;
            previousXOffset += Offset;
            previousChildSize = child.DesiredSize.Height;
            rcChild.Height = previousChildSize;
            rcChild.Width = Math.Max(arrangeSize.Width, child.DesiredSize.Width);

            child.Arrange(rcChild);
        }

        return arrangeSize;
    }

    protected override Size MeasureOverride(Size constraint)
    {
        var calculatedSize = base.MeasureOverride(constraint);
        calculatedSize.Width += Math.Abs(MaxOffset());
        return calculatedSize;
    }

    private IEnumerable<UIElement> EnumerateChildren() => Children.Enumerate();

    private double MaxOffset()
    {
        return Offset * EnumerateChildren().Count(x => x.Visibility == Visibility.Visible);
    }
}