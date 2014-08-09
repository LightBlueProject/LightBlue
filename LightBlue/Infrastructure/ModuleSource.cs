using System.Collections.Generic;

using Autofac;

using LightBlue.Hosted;

namespace LightBlue.Infrastructure
{
    public static class ModuleSource
    {
        private static readonly List<Module> _registeredModules;

        static ModuleSource()
        {
            _registeredModules = new List<Module>
            {
                new LightBlueHostedModule()
            };
        }

        public static IEnumerable<Module> RegisteredModules
        {
            get { return _registeredModules.AsReadOnly(); }
        }

        public static void AddModule(Module module)
        {
            _registeredModules.Add(module);
        }

        public static void ClearModules()
        {
            _registeredModules.Clear();
        }
    }
}