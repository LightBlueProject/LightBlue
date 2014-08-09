using System;

using Autofac;

using LightBlue.Infrastructure;

using Xunit;

namespace LightBlue.Tests
{
    public class ModuleSourceTests : IDisposable
    {
        [Fact]
        public void CanAddModule()
        {
            var testModule = new TestModule();

            ModuleSource.AddModule(testModule);

            Assert.Contains(testModule, ModuleSource.RegisteredModules);
        }

        [Fact]
        public void CanClearModules()
        {
            ModuleSource.AddModule(new TestModule());
            ModuleSource.AddModule(new TestModule());
            ModuleSource.AddModule(new TestModule());

            ModuleSource.ClearModules();

            Assert.Empty(ModuleSource.RegisteredModules);
        }

        public void Dispose()
        {
            ModuleSource.ClearModules();
        }
    }

    public class TestModule : Module
    { }
}