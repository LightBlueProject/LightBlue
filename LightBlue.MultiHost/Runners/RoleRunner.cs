using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public void Start()
        {


            if (IsWebSite())
            {
                var website = RunnerFactory.CreateForWebSite(_role);
                var role = RunnerFactory.CreateForWebRole(_role, _role.IsolationMode);
                _resources.Add(website);
                _resources.Add(role);
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
    }
}
