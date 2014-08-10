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
            var webHostArgs = WebHostArgs.ParseArgs(args);

            var runState = RunState.NotRun;
            while (runState.ShouldRunHost(webHostArgs.RetryMode))
            {
                runState = RunWebRole(webHostArgs);
            }
        }

        private static RunState RunWebRole(WebHostArgs webHostArgs)
        {
            try
            {
                using (var process = Process.Start(BuildProcessStartInfo(webHostArgs)))
                {
                    if (process == null)
                    {
                        Trace.TraceInformation("Existing process used.");
                        Task.Delay(-1).Wait();
                        return RunState.FailedToStart;
                    }

                    process.WaitForExit();

                    return process.StandardError.ReadToEnd().Trim().Length == 0
                        ? RunState.ExitedCleanly
                        : RunState.Failed;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToTraceMessage());
                return RunState.FailedToStart;
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