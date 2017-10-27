using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;

namespace LightBlue.Host
{
    public static class Program
    {
        [DllImport("user32.dll")]
        private static extern bool SetWindowText(IntPtr hWnd, string text);

        public static void Main(string[] args)
        {
            var hostArgs = HostArgs.ParseArgs(args);
            if (hostArgs == null)
            {
                return;
            }

            var handle = Process.GetCurrentProcess().MainWindowHandle;

            SetWindowText(handle, hostArgs.Title);

            var host = WorkerHostFactory.Create(hostArgs);

            host.Run(hostArgs.Assembly,
                hostArgs.ConfigurationPath,
                hostArgs.RoleName,
                hostArgs.UseHostedStorage);

            if (!hostArgs.AllowSilentFail)
            {
                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "The host {0} has exited unexpectedly",
                        hostArgs.Title));
            }
        }
    }
}