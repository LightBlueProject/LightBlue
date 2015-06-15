using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using LightBlue.Host;
using LightBlue.Infrastructure;
using LightBlue.Setup;
using LightBlue.WebHost;

namespace LightBlue.MultiWebHost
{
    public class NewRelicFilteringTraceListener : TraceListener
    {
        public override void Write(string message)
        {
            if (message.Contains("NewRelic")) return;
            Console.Write(message);
        }

        public override void WriteLine(string message)
        {
            if (message.Contains("NewRelic")) return;
            Console.WriteLine(message);
        }
    }

    public static class Program
    {
        private static readonly ConcurrentBag<string> AllRoleFolders = new ConcurrentBag<string>();
        static int _roleCount;

        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.AssemblyResolve += TryResolve;

            Console.Title = "Multi-Web-Host LightBlue emulator";

            var basePath = args[0];
            var configFile = args[1].Split(new [] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);

            Trace.Listeners.Add(new NewRelicFilteringTraceListener());
            AppDomain.CurrentDomain.UnhandledException +=
                UnhandledExceptionBehaviour.UnhandledExceptionHandler("Multi-web-host");

            foreach (var configLine in configFile)
            {

                var replacedConfigLine = configLine.Replace("$parentDirectory", basePath);
                var webHostArgs = WebHostArgs.ParseArgs(replacedConfigLine.Split(' '));
                if (webHostArgs == null)
                {
                    return;
                }

                try
                {
                    Console.Title = "Multi-Web-Host - hosting " + (++_roleCount) + " roles";

                    ConfigureWebConfig(webHostArgs);

                    RunWebSite(webHostArgs);
                    RunWebRole(webHostArgs).ContinueWith(t =>
                    {
                        if (!webHostArgs.AllowSilentFail)
                        {
                            throw new InvalidOperationException(
                                string.Format(
                                    CultureInfo.InvariantCulture,
                                    "The web host {0} has exited unexpectedly",
                                    webHostArgs.Title));
                        }
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToTraceMessage());
                }
            }

            Console.ReadLine();
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

        private static Task RunWebRole(WebHostArgs webHostArgs)
        {
            var hostRunner = new HostRunner();

            var roleDirectory = Path.GetDirectoryName(webHostArgs.Assembly);
            BootstrapLightBlueAssembly();
            AllRoleFolders.Add(roleDirectory);


            return Task.Factory.StartNew(
                () =>
                    hostRunner.Run(workerRoleAssembly: webHostArgs.Assembly,
                        configurationPath: webHostArgs.ConfigurationPath,
                        serviceDefinitionPath: webHostArgs.ServiceDefinitionPath,
                        roleName: webHostArgs.RoleName,
                        useHostedStorage: webHostArgs.UseHostedStorage), TaskCreationOptions.LongRunning);
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
            var manifestResourceStream = executingAssembly.GetManifestResourceStream("LightBlue.MultiWebHost.IISExpressConfiguration.template");
            if (manifestResourceStream == null)
            {
                throw new InvalidOperationException("Unable to retrieve IIS Express configuration template.");
            }

            var template = new StreamReader(manifestResourceStream).ReadToEnd();
            return template;
        }

        private static ProcessStartInfo BuildProcessStartInfo(WebHostArgs webHostArgs, string configurationFilePath)
        {
            string path = webHostArgs.Use64Bit
                ? Path.Combine(Environment.GetEnvironmentVariable("ProgramW6432"), @"IIS Express\iisexpress.exe")
                : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),@"IIS Express\iisexpress.exe");

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

        private static void BootstrapLightBlueAssembly()
        {
            LightBlueConfiguration.RunAsMultiHost();
        }

        private static Assembly TryResolve(object sender, ResolveEventArgs args)
        {
            foreach (var allLogFolder in AllRoleFolders)
            {
                if (File.Exists(Path.Combine(allLogFolder, new AssemblyName(args.Name).Name + ".dll")))
                    return Assembly.LoadFrom(Path.Combine(allLogFolder, new AssemblyName(args.Name).Name + ".dll"));
            }
            return null;
        }
    }
}