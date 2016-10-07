using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace LightBlue.MultiHost.Core.Runners
{
    public static class ThreadRunnerAssemblyCache
    {
        public static readonly string AssemblyCacheFolder = Path.Combine(Path.GetTempPath(), "LightBlue.MultiHost.AssemblyCache");

        public static void Initialise(IEnumerable<string> assemblyLocations)
        {
            if (Directory.Exists(AssemblyCacheFolder))
            {
                foreach (var f in Directory.EnumerateFiles(AssemblyCacheFolder))
                {
                    File.Delete(f);
                }
            }
            Directory.CreateDirectory(AssemblyCacheFolder);

            foreach (var path in assemblyLocations)
            {
                foreach (var asm in Directory.EnumerateFiles(path, "*.dll"))
                {
                    var target = Path.Combine(AssemblyCacheFolder, Path.GetFileName(asm));
                    if (!File.Exists(target)) File.Copy(asm, target);
                }
            }
        }

        static ThreadRunnerAssemblyCache()
        {
            AppDomain.CurrentDomain.AssemblyResolve += TryResolveAssembly;
        }

        private static Assembly TryResolveAssembly(object sender, ResolveEventArgs args)
        {
            var filename = new AssemblyName(args.Name).Name + ".dll";

            var filePath = Path.Combine(AssemblyCacheFolder, filename);
            if (File.Exists(filePath))
            {
                return Assembly.LoadFrom(filePath);
            }
            return null;
        }
    }
}