using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace LightBlue.MultiHost.Runners
{
    static class ThreadRunnerAssemblyCache
    {
        public static string AssemblyCacheFolder = Path.Combine(Path.GetTempPath(), "LightBlue.MultiHost.AssemblyCache");

        public static void Initialise(IEnumerable<string> assemblyLocations, string systemName)
        {
            var systemCacheFolder = Path.Combine(AssemblyCacheFolder, systemName);
            AssemblyCacheFolder = systemCacheFolder;

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
                foreach (var asm in Directory.EnumerateFiles(path, "*.dll").Concat(Directory.EnumerateFiles(path, "*.exe")))
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
            return LoadAssemblyIfExists(args.Name, ".dll")
                ?? LoadAssemblyIfExists(args.Name, ".exe");
        }

        private static Assembly LoadAssemblyIfExists(string name, string fileExtension)
        {
            var filename = new AssemblyName(name).Name + fileExtension;
            var filePath = Path.Combine(AssemblyCacheFolder, filename);

            return File.Exists(filePath) ? Assembly.LoadFrom(filePath) : null;
        }
    }
}