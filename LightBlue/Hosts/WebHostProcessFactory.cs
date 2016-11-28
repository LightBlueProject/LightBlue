using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using LightBlue.Infrastructure;
using LightBlue.Standalone;
using Serilog;

namespace LightBlue.Hosts
{
    public class WebHostProcessFactory
    {
        public static WebHostProcess Create(WebHost.Settings settings)
        {
            ScrubWebConfigAzureTraceListener(settings);

            var directory = CreateWorkingDirectory(settings);
            var applicationHost = WriteApplicationHostToDisk(settings, directory);
            var environment = CreateEnvironmentVariables(settings, directory);
            var arguments = CreateProcessArgs(applicationHost);

            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "IIS Express", "iisexpress.exe");
            if (!File.Exists(path))
                throw new FileNotFoundException("IISExpress not found at " + path);

            return new WebHostProcess(path, arguments, environment);
        }

        private static void ScrubWebConfigAzureTraceListener(WebHost.Settings settings)
        {
            var webConfig = new FileInfo(Path.Combine(settings.SiteDirectory, "web.config"));
            if (!webConfig.Exists)
                throw new FileNotFoundException("Web.config file not found at " + webConfig.FullName);

            ConfigurationManipulation.RemoveAzureTraceListenerFromConfiguration(webConfig.FullName);

            Log.Information("IISExpress removed azure trace listeners from web.config {WebConfig}", webConfig.FullName);
        }

        private static DirectoryInfo CreateWorkingDirectory(WebHost.Settings settings)
        {
            StandaloneEnvironment.LightBlueDataDirectory = @"c:\ProgramData\LightBlue";
            var processId = string.Format("{0}-iisexpress-{1}", settings.ServiceTitle, Process.GetCurrentProcess().Id);
            var directory = Directory.CreateDirectory(Path.Combine(StandaloneEnvironment.LightBlueDataDirectory, processId));
            Log.Information("IISExpress directory {IISExpressDirectory} created", directory.FullName);
            return directory;
        }

        private static string WriteApplicationHostToDisk(WebHost.Settings settings, DirectoryInfo directory)
        {
            var template = Resources.IISExpressTemplate
                .Replace("__SITEPATH__", settings.SiteDirectory)
                .Replace("__PROTOCOL__", "https")
                .Replace("__PORT__", settings.Port)
                .Replace("__HOSTNAME__", settings.Host);
            var applicationHost = Path.Combine(directory.FullName, "applicationhost.config");
            File.WriteAllText(applicationHost, template);

            Log.Information("IISExpress Application host config file created {ApplicationHost}", applicationHost);

            return applicationHost;
        }

        private static Dictionary<string, string> CreateEnvironmentVariables(WebHost.Settings settings, DirectoryInfo directory)
        {
            var cscfg = Path.IsPathRooted(settings.Cscfg)
                ? settings.Cscfg
                : Path.Combine(Environment.CurrentDirectory, settings.Cscfg);

            var csdef = Path.IsPathRooted(settings.Csdef)
                ? settings.Csdef
                : Path.Combine(Environment.CurrentDirectory, settings.Csdef);

            var environment = new Dictionary<string, string>();
            environment.Add("LightBlueHost", "true");
            environment.Add("LightBlueConfigurationPath", cscfg);
            environment.Add("LightBlueServiceDefinitionPath", csdef);
            environment.Add("LightBlueRoleName", settings.RoleName);
            environment.Add("LightBlueUseHostedStorage", "false");
            environment.Add("TMP", directory.FullName);
            environment.Add("TEMP", directory.FullName);

            foreach (var env in environment)
            {
                Log.Information("IISExpress environment variable {Name}={Variable}", env.Key, env.Value);
            }
            return environment;
        }

        private static string CreateProcessArgs(string applicationHost)
        {
            var sb = new StringBuilder();
            sb.AppendFormat(@" /config:""{0}""", applicationHost);
            sb.AppendFormat(@" /site:""{0}""", "LightBlue");
            sb.Append(" /trace:error");
            sb.Append(" /systray:true");
            var arguments = sb.ToString();
            return arguments;
        }
    }
}