using System;
using System.Globalization;
using System.Windows.Data;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.Resources.Icons;

namespace BuildNotifications.Resources.BuildTree.Converter;

internal class BuildReasonToIconTypeConverter : IValueConverter
{
    private BuildReasonToIconTypeConverter()
    {
    }

    public static BuildReasonToIconTypeConverter Instance { get; } = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is BuildReason asBuildReason)
        {
            switch (asBuildReason)
            {
                case BuildReason.CheckedIn:
                    return IconType.Ci;
                case BuildReason.Scheduled:
                    return IconType.Scheduled;
                case BuildReason.PullRequest:
                    return IconType.PullRequest;
                case BuildReason.Manual:
                    return IconType.ManualBuild;
                default:
                    return IconType.TriggeredBuild;
            }
        }

        return IconType.TriggeredBuild;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}