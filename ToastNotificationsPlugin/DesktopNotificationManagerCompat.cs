// ******************************************************************
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.
// ******************************************************************
// SEE https://docs.microsoft.com/en-us/windows/uwp/design/shell/tiles-and-notifications/send-local-toast-desktop

using System;
using System.Diagnostics;

namespace ToastNotificationsPlugin
{
    internal static class DesktopNotificationManagerCompat
    {
        /// <summary>
        /// If not running under the Desktop Bridge, you must call this method to register your AUMID with the Compat library and to
        /// register your COM CLSID and EXE in LocalServer32 registry. Feel free to call this regardless, and we will no-op if running
        /// under Desktop Bridge. Call this upon application startup, before calling any other APIs.
        /// </summary>
        /// <param name="aumid">An AUMID that uniquely identifies your application.</param>
        public static void RegisterAumidAndComServer<T>(string aumid)
        {
            if (string.IsNullOrWhiteSpace(aumid))
                throw new ArgumentException("You must provide an AUMID.", nameof(aumid));

            var exePath = Process.GetCurrentProcess().MainModule?.FileName;
            if (exePath != null)
                RegisterComServer<T>(exePath);
        }

        private static void RegisterComServer<T>(string exePath)
        {
            // We register the EXE to start up when the notification is activated
            var regString = $@"SOFTWARE\Classes\CLSID\{{{typeof(T).GUID}}}\LocalServer32";
            var key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(regString);

            // Include a flag so we know this was a toast activation and should wait for COM to process
            // We also wrap EXE path in quotes for extra security
            key?.SetValue(null, '"' + exePath + '"');
        }
    }
}