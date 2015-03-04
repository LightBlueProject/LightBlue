using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

using LightBlue.Host;
using LightBlue.Host.Stub;
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

                ConfigureWebConfig(webHostArgs);

                RunWebSite(webHostArgs);
                RunWebRole(webHostArgs);
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

        private static void ConfigureWebConfig(WebHostArgs webHostArgs)
        {
            var webConfigFilePath = DetermineWebConfigPath(webHostArgs);
            if (!File.Exists(webConfigFilePath))
            {
                throw new ArgumentException("No web.config could be located for the site");
            }

            ConfigurationManipulation.RemoveAzureTraceListenerFromConfiguration(webConfigFilePath);
        }

        private static void RunWebSite(WebHostArgs webHostArgs)
        {
            var iisExpressConfigurationFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".config");
            GenerateIisExpressConfigurationFile(webHostArgs, iisExpressConfigurationFilePath);

            using (var process = Process.Start(BuildProcessStartInfo(webHostArgs, iisExpressConfigurationFilePath)))
            {
                if (process == null)
                {
                    Console.WriteLine("Existing process used.");
                    return;
                }

                process.WaitForExit(2000);

                if (process.HasExited)
                {
                    Console.WriteLine(process.StandardError.ReadToEnd());
                }
            }
        }

        private static void RunWebRole(WebHostArgs webHostArgs)
        {
            var appDomainSetup = new AppDomainSetup
            {
                ApplicationBase = webHostArgs.SiteBinDirectory,
                ConfigurationFile = DetermineWebConfigPath(webHostArgs)
            };

            StubManagement.CopyStubAssemblyToRoleDirectory(webHostArgs.SiteBinDirectory);

            var appDomain = AppDomain.CreateDomain(
                "LightBlue",
                null,
                appDomainSetup);

            Trace.Listeners.Add(new ConsoleTraceListener());

            var stub = (HostStub) appDomain.CreateInstanceAndUnwrap(
                typeof(HostStub).Assembly.FullName,
                typeof(HostStub).FullName);

            stub.ConfigureTracing(new TraceShipper());

            stub.Run(workerRoleAssembly: webHostArgs.Assembly,
                configurationPath: webHostArgs.ConfigurationPath,
                serviceDefinitionPath: webHostArgs.ServiceDefinitionPath,
                roleName: webHostArgs.RoleName,
                useHostedStorage: webHostArgs.UseHostedStorage);
        }

        private static string DetermineWebConfigPath(WebHostArgs webHostArgs)
        {
            return Path.Combine(webHostArgs.SiteDirectory, "web.config");
        }

        private static void GenerateIisExpressConfigurationFile(WebHostArgs webHostArgs, string configurationFilePath)
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
            var manifestResourceStream = executingAssembly.GetManifestResourceStream("LightBlue.WebHost.IISExpressConfiguration.template");
            if (manifestResourceStream == null)
            {
                throw new InvalidOperationException("Unable to retrieve IIS Express configuration template.");
            }

            var template = new StreamReader(manifestResourceStream).ReadToEnd();
            return template;
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
    }
}