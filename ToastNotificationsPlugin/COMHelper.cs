/****************************** Module Header ******************************\
* Module Name:  COMHelper.cs
* Project:      CSExeCOMServer
* Copyright (c) Microsoft Corporation.
* 
* COMHelper provides the helper functions to register COM server
* and encapsulates the native COM APIs to be used in .NET.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Reflection;
using Microsoft.Win32;

namespace ToastNotificationsPlugin
{
    internal static class ComHelper
    {
        public static void RegisterLocalServer(Type t)
        {
            // Open the CLSID key of the component.
            using (var keyClsid = Registry.ClassesRoot.OpenSubKey(@"CLSID\" + t.GUID.ToString("B"), true))
            {
                if (keyClsid == null)
                    return;

                // try to delete key that might be auto generated
                try
                {
                    keyClsid.DeleteSubKeyTree("InprocServer32");
                }
                catch (Exception)
                {
                    // ignored
                }

                // Create "LocalServer32" under the CLSID key
                using (var subKey = keyClsid.CreateSubKey("LocalServer32"))
                {
                    subKey?.SetValue("", Assembly.GetExecutingAssembly().Location, RegistryValueKind.String);
                }
            }
        }
    }
}