using System.Diagnostics;
using System.IO;
using LightBlue.MultiHost.ViewModel;

namespace LightBlue.MultiHost
{
    public static class RoleExtensions
    {
        public static Process NewDefaultProcess(this Role role, string file, string args)
        {
            var psi = new ProcessStartInfo(file, args)
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = Path.GetDirectoryName(role.Config.ConfigurationPath)
            };

            psi.EnvironmentVariables.Add("LightBlueHost", "true");
            psi.EnvironmentVariables.Add("LightBlueConfigurationPath", role.Config.ConfigurationPath);
            psi.EnvironmentVariables.Add("LightBlueRoleName", role.Config.RoleName);
            psi.EnvironmentVariables.Add("LightBlueUseHostedStorage", "true");

            return new Process
            {
                EnableRaisingEvents = true,
                StartInfo = psi
            };
        }
    }
}
