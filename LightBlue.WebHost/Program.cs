using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;

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
            var iisExpressConfigurationFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".config");
            GenerateIisExpressConfigurationFile(webHostArgs, iisExpressConfigurationFilePath);

            var webConfigFilePath = Path.Combine(webHostArgs.SiteDirectory, "web.config");
            if (!File.Exists(webConfigFilePath))
            {
                throw new ArgumentException("No web.config could be located for the site");
            }

            ConfigurationManipulation.RemoveAzureTraceListenerFromConfiguration(webConfigFilePath);

            using (var process = Process.Start(BuildProcessStartInfo(webHostArgs, iisExpressConfigurationFilePath)))
            {
                if (process == null)
                {
                    Console.WriteLine("Existing process used.");
                    return;
                }

                process.WaitForExit(3000);

                if (process.HasExited)
                {
                    Console.WriteLine(process.StandardError.ReadToEnd());
                }
            }
        }

        private static void GenerateIisExpressConfigurationFile(WebHostArgs webHostArgs, string configurationFilePath)
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            var manifestResourceStream = executingAssembly.GetManifestResourceStream("LightBlue.WebHost.IISExpressConfiguration.template");
            if (manifestResourceStream == null)
            {
                throw new InvalidOperationException("Unable to retrieve IIS Express configuration template.");
            }

            var template = new StreamReader(manifestResourceStream).ReadToEnd();
            template = template.Replace("__SITEPATH__", webHostArgs.SiteDirectory);
            template = template.Replace("__PROTOCOL__", webHostArgs.UseSsl ? "https" : "http");
            template = template.Replace("__PORT__", webHostArgs.Port.ToString(CultureInfo.InvariantCulture));
            template = template.Replace("__HOSTNAME__", webHostArgs.Hostname);

            File.WriteAllText(configurationFilePath, template);
        }

        private static ProcessStartInfo BuildProcessStartInfo(WebHostArgs webHostArgs, string configurationFilePath)
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"IIS Express\iisexpress.exe"),
                Arguments = string.Format(
                    CultureInfo.InvariantCulture,
                    "/config:\"{0}\" /site:LightBlue",
                    configurationFilePath),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            processStartInfo.EnvironmentVariables.Add("LightBlueHost", "true");
            processStartInfo.EnvironmentVariables.Add("LightBlueConfigurationPath", webHostArgs.ConfigurationPath);
            processStartInfo.EnvironmentVariables.Add("LightBlueServiceDefinitionPath", webHostArgs.ServiceDefinitionPath);
            processStartInfo.EnvironmentVariables.Add("LightBlueRoleName", webHostArgs.RoleName);

            return processStartInfo;
        }
    }
}