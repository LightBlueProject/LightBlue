using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using LightBlue.MultiHost.ViewModel;

namespace LightBlue.MultiHost.Runners
{
    public class RoleRunner : IRunner
    {
        private readonly Role _role;

        private readonly List<IRunner> _resources = new List<IRunner>();

        public Task Started
        {
            get
            {
                var tasks = _resources.Select(x => x.Started).ToArray();
                return Task.WhenAll(tasks);
            }
        }

        public Task Completed // == Crashed
        {
            get { return CreateFriendlyTask(); }
        }

        public string Identifier { get { return "Role Runner: " + _role.Config.Title; } }

        private async Task CreateFriendlyTask()
        {
            var tasks = _resources.Select(x => x.Completed).ToArray();
            await await Task.WhenAny(tasks);
        }

        public RoleRunner(Role role)
        {
            _role = role;
        }

        private bool IsWebSite()
        {
            return !string.IsNullOrWhiteSpace(_role.Config.Hostname);
        }

        public IEnumerable<string> RunnerIdentifiers { get { return _resources.Select(r => r.Identifier); }}

        public void Start()
        {
            if (IsWebSite())
            {
                var website = RunnerFactory.CreateForWebSite(_role);
                var role = RunnerFactory.CreateForWebRole(_role, _role.IsolationMode);
                _resources.Add(role);
                _resources.Add(website);
                website.Start();
                role.Start();
            }
            else
            {
                var role = RunnerFactory.CreateForWorkerRole(_role, _role.IsolationMode);
                _resources.Add(role);
                role.Start();
            }
        }

        public void Dispose()
        {
            foreach (var r in _resources)
            {
                r.Dispose();
            }
        }

        public void Debug()
        {
            if (IsWebSite())
            {
                if (_resources.OfType<IisExpressRunner>().First().Debug())
                {
                    return;
                }
            }

            Debugger.Launch();
        }
    }
}
