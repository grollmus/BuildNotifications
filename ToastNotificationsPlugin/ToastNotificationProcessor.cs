using System;
using System.IO;
using Windows.UI.Notifications;
using BuildNotifications.PluginInterfacesLegacy.Notification;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using MS.WindowsAPICodePack.Internal;

namespace ToastNotificationsPlugin
{
    public class ToastNotificationProcessor : INotificationProcessor
    {
        private ToastNotificationFactory _toastNotificationFactory;
        internal const string ApplicationId = "github.com.grollmus.BuildNotifications";

        public void Initialize()
        {
            TryCreateShortcut();
            _toastNotificationFactory = new ToastNotificationFactory();

            ToastNotificationManager.History.Clear(ApplicationId);
        }

        // In order to display toasts, a desktop application must have a shortcut on the Start menu.
        // Also, an AppUserModelID must be set on that shortcut.
        private static void TryCreateShortcut()
        {
#if DEBUG
            return;
#endif
#pragma warning disable 162
            // ReSharper disable HeuristicUnreachableCode
            var shortcutPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Microsoft\\Windows\\Start Menu\\Programs\\BuildNotifications.lnk";
            File.Delete(shortcutPath);
            if (File.Exists(shortcutPath))
                return;

            InstallShortcut(shortcutPath);
            // ReSharper enable HeuristicUnreachableCode
#pragma warning restore 162
        }

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

            using (PropVariant appId = new PropVariant(ApplicationId))
            {
                using (var activatorGuid = new PropVariant(activatorGuidAsString))
                {
                    ErrorHelper.VerifySucceeded(newShortcutProperties.SetValue(SystemProperties.System.AppUserModel.ID, appId));
                    ErrorHelper.VerifySucceeded(newShortcutProperties.SetValue(ref toastId, activatorGuid));
                    ErrorHelper.VerifySucceeded(newShortcutProperties.Commit());
                }
            }

            // Commit the shortcut to disk
            var newShortcutSave = (IPersistFile) newShortcut;

            ErrorHelper.VerifySucceeded(newShortcutSave.Save(shortcutPath, true));
            // ReSharper enable SuspiciousTypeConversion.Global
        }

        public void Process(IDistributedNotification notification) => _toastNotificationFactory.Process(notification);

        public void Shutdown()
        {
            // nothing to do    
        }
    }
}