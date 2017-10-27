using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using LightBlue.Infrastructure;

namespace LightBlue.WebHost
{
    public static class Program
    {
        [DllImport("user32.dll")]
        private static extern bool SetWindowText(IntPtr hWnd, string text);

        public static void Main(string[] args)
        {
            var webHostArgs = WebHostArgs.ParseArgs(args);
            if (webHostArgs == null)
            {
                return;
            }

            try
            {
                var handle = Process.GetCurrentProcess().MainWindowHandle;

                SetWindowText(handle, webHostArgs.Title);

                var host = WebHostFactory.Create(webHostArgs);

                host.Run(webHostArgs.Assembly,
                    webHostArgs.ConfigurationPath,
                    webHostArgs.RoleName,
                    webHostArgs.UseHostedStorage);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToTraceMessage());
            }

            if (!webHostArgs.AllowSilentFail)
            {
                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "The web host {0} has exited unexpectedly",
                        webHostArgs.Title));
            }
        }
    }
}