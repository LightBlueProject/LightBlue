using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Windows;

using LightBlue.Infrastructure;
using LightBlue.MultiHost.Core.Configuration;
using LightBlue.MultiHost.Core.IISExpress;
using LightBlue.MultiHost.Core.Runners;
using LightBlue.Setup;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace LightBlue.MultiHost
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                var filepath = IsMissingConfigurationFilePath(e) 
                    ? ShowOpenConfigurationFileDialog() 
                    : e.Args.Single();

                if (IsRunningDemoMode(filepath))
                {
                    var configuration = MultiHostConfiguration.Default;
                    MultiHostRoot.Configure(configuration);
                }
                else
                {
                    var configuration = MultiHostConfiguration.Load(filepath);
                    MultiHostRoot.Configure(configuration);
                    MultiHostRoot.Start();
                }

                base.OnStartup(e);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not start multi-host: " + ex.ToTraceMessage());
            }
        }

        private static bool IsMissingConfigurationFilePath(StartupEventArgs e)
        {
            return e.Args.Length != 1;
        }

        private static string ShowOpenConfigurationFileDialog()
        {
            var dialog = new OpenFileDialog
            {
                Title = "LightBlue MultiHost: please select multi-host configuration file (.json)",
                Filter = "MultiHost Configuration Files (.json)|*.json",
                CheckFileExists = true
            };
            return dialog.ShowDialog().GetValueOrDefault() 
                ? dialog.FileName 
                : null;
        }

        private static bool IsRunningDemoMode(string filepath)
        {
            return string.IsNullOrWhiteSpace(filepath);
        }
    }    
}
