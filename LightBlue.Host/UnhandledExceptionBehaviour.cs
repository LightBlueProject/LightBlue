using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

using LightBlue.Infrastructure;

namespace LightBlue.Host
{
    public static class UnhandledExceptionBehaviour
    {
        private const UInt32 FLASHW_STOP = 0;
        public const UInt32 FLASHW_ALL = 3;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool FlashWindowEx(ref FLASHWINFO pwfi);

        [StructLayout(LayoutKind.Sequential)]
        public struct FLASHWINFO
        {
            public UInt32 cbSize;
            public IntPtr hwnd;
            public UInt32 dwFlags;
            public UInt32 uCount;
            public Int32 dwTimeout;
        }

        public static UnhandledExceptionEventHandler UnhandledExceptionHandler(string title)
        {
            return (sender, eventArgs) =>
            {
                FlashWindow(FLASHW_ALL);

                var originalColours = SetEmphasisConsoleColours();

                Console.WriteLine(
                    "The hosted application {0} has thrown an unhandled exception",
                    title);
                Console.WriteLine();
                Console.WriteLine("Options:");
                Console.WriteLine("Press 'x' to kill the process without viewing the error");
                Console.WriteLine("Press 'd' to launch the debugger");
                Console.WriteLine("Press 't' to throw the exception");
                Console.WriteLine("Press anything else to write the exception to the console and exit");

                var option = Console.ReadKey();

                FlashWindow(FLASHW_STOP);
                Console.WriteLine();

                switch (option.KeyChar)
                {
                    case 'x':
                    case 'X':
                        RestoreConsoleColours(originalColours);
                        Environment.Exit(1);
                        return;
                    case 'd':
                    case 'D':
                        if (Debugger.IsAttached)
                        {
                            Debugger.Break();
                        }
                        else
                        {
                            Debugger.Launch();
                        }
                        break;
                    case 't':
                    case 'T':
                        RestoreConsoleColours(originalColours);
                        return;
                    default:
                        var exception = eventArgs.ExceptionObject as Exception;
                        if (exception == null)
                        {
                            Console.WriteLine("Unhandled exception cannot be cast to System.Exception");
                            if (eventArgs.ExceptionObject != null)
                            {
                                Console.WriteLine(eventArgs.ExceptionObject.ToString());
                            }
                        }
                        else
                        {
                            Console.WriteLine(exception.ToTraceMessage());
                        }
                        RestoreConsoleColours(originalColours);
                        Environment.Exit(1);
                        break;
                }
                RestoreConsoleColours(originalColours);
            };
        }

        private static void RestoreConsoleColours(ConsoleColor[] originalColours)
        {
            Console.ForegroundColor = originalColours[0];
            Console.BackgroundColor = originalColours[1];
        }

        private static ConsoleColor[] SetEmphasisConsoleColours()
        {
            var originalColours = new[] {Console.ForegroundColor, Console.BackgroundColor};
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Red;
            return originalColours;
        }

        private static void FlashWindow(uint flags)
        {
            var fInfo = new FLASHWINFO();

            fInfo.cbSize = Convert.ToUInt32(Marshal.SizeOf(fInfo));
            fInfo.hwnd = Process.GetCurrentProcess().MainWindowHandle;
            fInfo.dwFlags = flags;
            fInfo.uCount = UInt32.MaxValue;
            fInfo.dwTimeout = 0;

            FlashWindowEx(ref fInfo);
        }
    }
}