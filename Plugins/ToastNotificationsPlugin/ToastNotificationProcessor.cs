using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Windows.UI.Notifications;
using BuildNotifications.PluginInterfaces.Notification;
using JetBrains.Annotations;
using Microsoft.WindowsAPICodePack.PropertySystem;
using Microsoft.WindowsAPICodePack.Win32Native.PropertySystem;

namespace ToastNotificationsPlugin
{
    [UsedImplicitly]
    [ExcludeFromCodeCoverage]
    public class ToastNotificationProcessor : INotificationProcessor
    {
        // Only used in DEBUG
        // ReSharper disable once UnusedMember.Local
        private static void InstallShortcut(string shortcutPath)
        {
            // ReSharper disable SuspiciousTypeConversion.Global
            // Find the path to the current executable
            var exePath = System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName;
            var newShortcut = (IShellLinkW) new CShellLink();

            // Create a shortcut to the exe
            ErrorHelper.VerifySucceeded(newShortcut.SetPath(exePath));
            ErrorHelper.VerifySucceeded(newShortcut.SetArguments(""));

            // Open the shortcut property store, set the AppUserModelId property
            var newShortcutProperties = (IPropertyStore) newShortcut;

            var activatorGuidAsString = typeof(PluginNotificationActivator).GUID.ToString();
            var toastId = new PropertyKey(Guid.Parse($"{{{activatorGuidAsString}}}"), 26);

            using (var appId = new PropVariant(ApplicationId))
            {
                using (var activatorGuid = new PropVariant(activatorGuidAsString))
                {
                    ErrorHelper.VerifySucceeded(newShortcutProperties.SetValue(Microsoft.WindowsAPICodePack.COMNative.Shell.PropertySystem.SystemProperties.System.AppUserModel.ID, appId));
                    ErrorHelper.VerifySucceeded(newShortcutProperties.SetValue(ref toastId, activatorGuid));
                    ErrorHelper.VerifySucceeded(newShortcutProperties.Commit());
                }
            }

            // Commit the shortcut to disk
            var newShortcutSave = (IPersistFile) newShortcut;

            ErrorHelper.VerifySucceeded(newShortcutSave.Save(shortcutPath, true));
            // ReSharper enable SuspiciousTypeConversion.Global
        }

        // In order to display toasts, a desktop application must have a shortcut on the Start menu.
        // Also, an AppUserModelID must be set on that shortcut.
        private static void TryCreateShortcut()
        {
            // for releases, the installer creates the shortcut. So this is only for debug purposes.
#if DEBUG
            var shortcutPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Microsoft\\Windows\\Start Menu\\Programs\\BuildNotifications.lnk";
            File.Delete(shortcutPath);
            if (File.Exists(shortcutPath))
                return;

            InstallShortcut(shortcutPath);
#endif
        }

        public void Initialize()
        {
            TryCreateShortcut();
            _toastNotificationFactory = new ToastNotificationFactory();

            ToastNotificationManager.History.Clear(ApplicationId);
        }

        private ToastNotificationFactory? _toastNotificationFactory;
        internal const string ApplicationId = "github.com.grollmus.BuildNotifications";
        internal const string Group = "BuildNotifications";

        public void Process(IDistributedNotification notification)
        {
            _toastNotificationFactory?.Process(notification);
        }

        public void Clear(IDistributedNotification notification)
        {
            var tag = _toastNotificationFactory?.NotificationTag(notification);

            var notifications = ToastNotificationManager.History.GetHistory(ApplicationId).ToList();
            if (notifications.Any(n => n.Tag == tag))
            {
                try
                {
                    ToastNotificationManager.History.Remove(tag, Group, ApplicationId);
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }

        public void Shutdown()
        {
            // nothing to do
        }
    }
}