using System;
using System.Collections.Generic;
using System.IO;

using NDesk.Options;

namespace LightBlue.Host
{
    public class HostArgs
    {
        public string Assembly { get; private set; }
        public RetryMode RetryMode { get; private set; }

        public static HostArgs ParseArgs(IEnumerable<string> args)
        {
            string assembly = null;
            var retryMode = RetryMode.Infinite;

            var options = new OptionSet
            {
                {"a|assembly=", "", v => assembly = v},
                {"r|retryMode=", "", v => { retryMode = (RetryMode) Enum.Parse(typeof(RetryMode), v, true); }},
            };

            options.Parse(args);

            if (string.IsNullOrWhiteSpace(assembly))
            {
                throw new ArgumentException("Host requires an assembly to run.");
            }

            var roleAssemblyAbsolutePath = Path.IsPathRooted(assembly)
                ? assembly
                : Path.Combine(Environment.CurrentDirectory, assembly);

            if (!File.Exists(roleAssemblyAbsolutePath))
            {
                throw new FileNotFoundException("The specified assembly cannot be found. The assembly must be in the host directory or be specified as an absolute path.");
            }

            return new HostArgs
            {
                Assembly = assembly, RetryMode = retryMode
            };
        }
    }
}