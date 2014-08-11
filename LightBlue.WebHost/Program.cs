using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

using LightBlue.Infrastructure;

namespace LightBlue.WebHost
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                var webHostArgs = WebHostArgs.ParseArgs(args);

                RunWebRole(webHostArgs);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToTraceMessage());
            }
        }

        private static void RunWebRole(WebHostArgs webHostArgs)
        {
            using (var process = Process.Start(BuildProcessStartInfo(webHostArgs)))
            {
                if (process != null)
                {
                    return;
                }

                Console.WriteLine("Existing process used.");
                Task.Delay(-1).Wait();
            }
        }

        private static ProcessStartInfo BuildProcessStartInfo(WebHostArgs webHostArgs)
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"IIS Express\iisexpress.exe"),
                Arguments = string.Format(
                    CultureInfo.InvariantCulture,
                    "/path:\"{0}\" /port:{1}",
                    webHostArgs.SiteDirectory,
                    webHostArgs.Port),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            processStartInfo.EnvironmentVariables.Add("LightBlueHost", "true");
            processStartInfo.EnvironmentVariables.Add("LightBlueConfigurationPath", webHostArgs.ConfigurationPath);
            processStartInfo.EnvironmentVariables.Add("LightBlueRoleName", webHostArgs.RoleName);

            return processStartInfo;
        }
    }
}