using Spectre.Console;
using Spectre.Console.Cli;
using System;
using System.ComponentModel;
using System.IO;

namespace LightBlue.Host
{
    internal sealed class HostAssemblyCommand : Command<HostAssemblyCommand.Settings>
    {
        public sealed class Settings : CommandSettings
        {
            [Description("The path to the primary assembly for the worker role.")]
            [CommandOption("-a|--assembly <assembly>")]
            public string Assembly { get; set; }

            [Description("The name of the role as defined in the configuration file.")]
            [CommandOption("-n|--roleName <roleName>")]
            public string RoleName { get; set; }

            [Description("Optional title for the role window. Defaults to role name if not specified.")]
            [CommandOption("-t|--serviceTitle <serviceTitle>")]
            public string ServiceTitle { get; set; }

            [Description("The path to the configuration file. Usually '<assemblyname>.dll.config'.")]
            [CommandOption("-c|--configurationPath <configurationPath>")]
            public string ConfigurationPath { get; set; }

            [Description("Use hosted storage (Emulator/Actual Azure) inside the LightBlue host.")]
            [CommandOption("--useHostedStorage")]
            public bool UseHostedStorage { get; set; }

            [Description("Allow the host to fail silently instead of throwing an exception when the hosted process exits.")]
            [CommandOption("--allowSilentFail")]
            public bool AllowSilentFail { get; set; }

            public string RoleAssemblyAbsolutePath => Path.IsPathRooted(Assembly)
                ? Assembly
                : Path.Combine(Environment.CurrentDirectory, Assembly);

            public string RoleConfigurationFile => Assembly + ".config";

            public string WindowTitle => string.IsNullOrWhiteSpace(ServiceTitle)
                ? RoleName
                : ServiceTitle;

            public string ApplicationBase => Path.GetDirectoryName(Assembly);
        }

        public override ValidationResult Validate(CommandContext context, Settings settings)
        {
            if (string.IsNullOrWhiteSpace(settings.Assembly))
                return ValidationResult.Error("Missing required argument 'assembly'.");

            if (string.IsNullOrWhiteSpace(settings.RoleName))
                return ValidationResult.Error("Missing required argument 'roleName'.");
            
            if (string.IsNullOrWhiteSpace(settings.ConfigurationPath))
                return ValidationResult.Error("Missing required argument 'configurationPath'.");

            if (!File.Exists(settings.RoleAssemblyAbsolutePath))
                return ValidationResult.Error($"The specified assembly '{settings.RoleAssemblyAbsolutePath}' cannot be found. The assembly must be in the host directory or be specified as an absolute path.");

            if (!File.Exists(settings.RoleConfigurationFile))
                return ValidationResult.Error($"The configuration file '{settings.RoleConfigurationFile}' for the role cannot be located.");

            return ValidationResult.Success();
        }

        public override int Execute(CommandContext context, Settings settings)
        {
            OSWindowManager.SetWindowTitle(settings.WindowTitle);

            try
            {
                var host = WorkerHostFactory.Create(settings);
                host.Run(settings.Assembly,
                    settings.ConfigurationPath,
                    settings.RoleName,
                    settings.UseHostedStorage);
            }
            catch (Exception e)
            {
                AnsiConsole.WriteException(e);
                return 1;
            }

            if (!settings.AllowSilentFail)
            {
                throw new InvalidOperationException($"The host '{settings.WindowTitle}' has exited unexpectedly");
            }

            return 0;
        }
    }
}