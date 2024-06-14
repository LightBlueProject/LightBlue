using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace LightBlue.Host
{
    internal static class OSWindowManager
    {
        [DllImport("user32.dll")]
        private static extern bool SetWindowText(IntPtr hWnd, string text);

        public static void SetWindowTitle(string windowTitle)
        {
            var handle = Process.GetCurrentProcess().MainWindowHandle;
            SetWindowText(handle, windowTitle);
        }
    }
}