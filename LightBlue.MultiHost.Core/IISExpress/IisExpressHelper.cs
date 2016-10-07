using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace LightBlue.MultiHost.Core.IISExpress
{
    public static class IisExpressHelper
    {
        public static void KillIisExpressProcesses()
        {
            foreach (var p in Process.GetProcessesByName("iisexpress"))
            {
                p.Kill();
            }
        }

        public static ProcessStartInfo BuildProcessStartInfo(WebHostArgs webHostArgs, string configurationFilePath)
        {
            string path = webHostArgs.Use64Bit
                ? Path.Combine(Environment.GetEnvironmentVariable("ProgramW6432"), @"IIS Express\iisexpress.exe")
                : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"IIS Express\iisexpress.exe");

            var processStartInfo = new ProcessStartInfo
            {
                FileName = path,
                Arguments = string.Format(
                    CultureInfo.InvariantCulture,
                    "/config:\"{0}\" /site:LightBlue",
                    configurationFilePath),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            processStartInfo.EnvironmentVariables.Add("LightBlueHost", "true");
            processStartInfo.EnvironmentVariables.Add("LightBlueConfigurationPath", webHostArgs.ConfigurationPath);
            processStartInfo.EnvironmentVariables.Add("LightBlueServiceDefinitionPath", webHostArgs.ServiceDefinitionPath);
            processStartInfo.EnvironmentVariables.Add("LightBlueRoleName", webHostArgs.RoleName);
            processStartInfo.EnvironmentVariables.Add("LightBlueUseHostedStorage", webHostArgs.UseHostedStorage.ToString());

            var processId = webHostArgs.RoleName
                            + "-web-"
                            + Process.GetCurrentProcess().Id.ToString(CultureInfo.InvariantCulture);

            var temporaryDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "LightBlue",
                "temp",
                processId);

            Directory.CreateDirectory(temporaryDirectory);
            processStartInfo.EnvironmentVariables["TMP"] = temporaryDirectory;
            processStartInfo.EnvironmentVariables["TEMP"] = temporaryDirectory;

            return processStartInfo;
        }
        
        public static void GenerateIisExpressConfigurationFile(WebHostArgs webHostArgs, string configurationFilePath)
        {
            var template = ObtainIisExpressConfigurationTemplate(webHostArgs);

            template = template.Replace("__SITEPATH__", webHostArgs.SiteDirectory);
            template = template.Replace("__PROTOCOL__", webHostArgs.UseSsl ? "https" : "http");
            template = template.Replace("__PORT__", webHostArgs.Port.ToString(CultureInfo.InvariantCulture));
            template = template.Replace("__HOSTNAME__", webHostArgs.Hostname);

            File.WriteAllText(configurationFilePath, template);
        }

        private static string ObtainIisExpressConfigurationTemplate(WebHostArgs webHostArgs)
        {
            if (!string.IsNullOrWhiteSpace(webHostArgs.IisExpressTemplate))
            {
                return File.ReadAllText(webHostArgs.IisExpressTemplate);
            }

            var executingAssembly = Assembly.GetExecutingAssembly();
            var manifestResourceStream = executingAssembly.GetManifestResourceStream("LightBlue.MultiHost.Core.IISExpress.Configuration.template");
            if (manifestResourceStream == null)
            {
                throw new InvalidOperationException("Unable to retrieve IIS Express configuration template.");
            }

            var template = new StreamReader(manifestResourceStream).ReadToEnd();
            return template;
        }
    }
}
