using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text.Json;
using System.Windows;

using LightBlue.Infrastructure;
using LightBlue.MultiHost.Configuration;
using LightBlue.MultiHost.IISExpress;
using LightBlue.MultiHost.Runners;
using LightBlue.Setup;
using Microsoft.Win32;

namespace LightBlue.MultiHost
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static MultiHostConfiguration Configuration { get; private set; }

        public static string MultiHostConfigurationFilePath { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                MultiHostConfigurationFilePath = null;

                if (e.Args.Length != 1)
                {
                    var d = new OpenFileDialog();
                    d.Title = "LightBlue MultiHost: please select multi-host configuration file (.json)";
                    d.Filter = "MultiHost Configuration Files (.json)|*.json";
                    d.CheckFileExists = true;
                    if (d.ShowDialog().GetValueOrDefault())
                    {
                        MultiHostConfigurationFilePath = d.FileName;

                    }
                }
                else
                {
                    MultiHostConfigurationFilePath = e.Args.Single();
                }

                if (string.IsNullOrWhiteSpace(MultiHostConfigurationFilePath))
                {
                    Configuration = new MultiHostConfiguration
                    {
                        Roles = new[]
                        {
                            new RoleConfiguration {Title = "Demo Web Site", RoleName = "WebRole"},
                            new RoleConfiguration
                            {
                                Title = "Demo Web Site 2",
                                RoleName = "WebRole",
                                RoleIsolationMode = "AppDomain"
                            },
                            new RoleConfiguration {Title = "Demo Domain", RoleName = "CommandProcessor"},
                            new RoleConfiguration
                            {
                                Title = "Demo Domain 2",
                                RoleName = "ReadModelPopulator",
                                RoleIsolationMode = "AppDomain"
                            }
                        },
                    };
                }
                else
                {
                    var configDir = Path.GetDirectoryName(MultiHostConfigurationFilePath);
                    var json = File.ReadAllText(MultiHostConfigurationFilePath);
                    Configuration = JsonSerializer.Deserialize<MultiHostConfiguration>(json);

                    foreach (var c in Configuration.Roles)
                    {
                        c.ConfigurationPath = Path.GetFullPath(Path.Combine(configDir, c.ConfigurationPath));
                        c.Assembly = Path.GetFullPath(Path.Combine(configDir, c.Assembly));
                    }

                    var query =
                        from c in Configuration.Roles
                        let relativePath = c.Assembly.ToLowerInvariant().EndsWith(".dll")
                                        || c.Assembly.ToLowerInvariant().EndsWith(".exe")
                            ? Path.GetDirectoryName(c.Assembly)
                            : c.Assembly
                        select relativePath;

                    var assemblyLocations = query.ToArray();

                    ThreadRunnerAssemblyCache.Initialise(assemblyLocations, Configuration.SystemName);
                    IisExpressHelper.KillIisExpressProcesses();
                    LightBlueConfiguration.SetAsMultiHost();
                }

                base.OnStartup(e);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not start multi-host: " + ex.ToTraceMessage());
            }
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            var parent = Process.GetCurrentProcess();
            var children = parent.GetChildren();
            foreach (var c in children)
            {
                foreach (var p in c.GetChildren())
                {
                    if (!p.HasExited)
                    {
                        p.Kill();
                    }
                }
                if (!c.HasExited)
                {
                    c.Kill();
                }
            }
        }
    }    
}
