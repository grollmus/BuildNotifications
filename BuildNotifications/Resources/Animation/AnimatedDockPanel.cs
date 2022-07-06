using System.Windows;
using System.Windows.Controls;

namespace BuildNotifications.Resources.Animation;

internal class AnimatedDockPanel : DockPanel
{
    public AnimatedDockPanel()
    {
        _helper = new AnimatedPanelHelper();
    }

    public double AnimationDuration
    {
        get => _helper.AnimationDuration;
        set => _helper.AnimationDuration = value;
    }

    public double Delay
    {
        get => _helper.Delay;
        set => _helper.Delay = value;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var positions = _helper.StoreOldPositions(this);
        var arrangedSize = base.ArrangeOverride(finalSize);
        _helper.AnimateToNewPositions(positions, this);
        return arrangedSize;
    }

    private readonly AnimatedPanelHelper _helper;
}