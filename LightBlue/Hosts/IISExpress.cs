using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Serilog;

namespace LightBlue.Hosts
{
    public class IISExpress
    {
        public static ProcessHost Start(WebHost.Settings settings, Action<string> output)
        {
            var processId = string.Format("{0}-iisexpress-{1}", settings.ServiceTitle, Process.GetCurrentProcess().Id);
            var local = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var directory = Directory.CreateDirectory(Path.Combine(local, "LightBlue", "temp", processId));

            Log.Information("IIS Express directory {IISExpressDirectory} created", directory.FullName);

            var template = Resources.IISExpressTemplate
                .Replace("__SITEPATH__", settings.SiteDirectory)
                .Replace("__PROTOCOL__", "https")
                .Replace("__PORT__", settings.Port)
                .Replace("__HOSTNAME__", settings.Host);
            var applicationHost = Path.Combine(directory.FullName, "applicationhost.config");

            File.WriteAllText(applicationHost, template);

            Log.Information("IIS Express Application host config created at {ApplicationHost}", applicationHost);

            var sb = new StringBuilder();
            sb.Append(string.Format(@" /config:""{0}""", applicationHost));
            sb.Append(string.Format(@" /site:""{0}""", "LightBlue"));
            sb.Append(" /trace:error");
            sb.Append(" /systray:true");
            var arguments = sb.ToString();

            var environment = new Dictionary<string, string>();
            environment.Add("LightBlueHost", "true");
            environment.Add("LightBlueConfigurationPath", settings.Cscfg);
            environment.Add("LightBlueServiceDefinitionPath", settings.Csdef);
            environment.Add("LightBlueRoleName", settings.RoleName);
            environment.Add("LightBlueUseHostedStorage", "false");
            environment.Add("TMP", directory.FullName);
            environment.Add("TEMP", directory.FullName);

            Log.Information("LightBlue context IIS express environment variables {@IISExpressEnvironmentVariables}", environment);

            return new ProcessHost(@"c:\program files (x86)\IIS Express\IISExpress.exe", arguments, environment, output);
        }
    }
}