using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LightBlue.MultiHost.Core.Runners
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

        public IEnumerable<string> RunnerIdentifiers { get { return _resources.Select(r => r.Identifier); }}

        public void Start()
        {
            if (_role.IsIisExpress)
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

        public void DebugIisExpress()
        {
            var iisExpressRunner = _resources.OfType<IisExpressRunner>().FirstOrDefault();
            if (iisExpressRunner != null)
            {
                iisExpressRunner.Debug();
                
            }
            else
            {
                throw new NotSupportedException("Role is not running in IIS Express");
            }
        }
    }
}
