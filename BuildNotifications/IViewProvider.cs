using System.Windows;

namespace BuildNotifications;

public interface IViewProvider
{
    Window View { get; }
}