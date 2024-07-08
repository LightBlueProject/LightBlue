using Spectre.Console.Cli;
using System;
using System.Runtime.InteropServices;

namespace LightBlue.Host
{
    public static class Program
    {
        [DllImport("user32.dll")]
        private static extern bool SetWindowText(IntPtr hWnd, string text);

        public static int Main(string[] args)
        {
            var app = new CommandApp<HostAssemblyCommand>();
            app.Configure(config => config.UseStrictParsing());
            return app.Run(args);
        }
    }
}