using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LightBlue.MultiHost.Configuration;
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

        public IEnumerable<string> RunnerIdentifiers { get { return _resources.Select(r => r.Identifier); } }

        public void Start()
        {
            IRunner role;
            switch (_role.RunnerType)
            {
                case RunnerType.DotNetFramework:
                    role = RunnerFactory.CreateDotNetFrameworkRunner(_role);
                    break;
                case RunnerType.DotNetCore:
                    role = RunnerFactory.CreateDotNetCoreRunner(_role);
                    break;
                case RunnerType.Node:
                    role = RunnerFactory.CreateNpmRunner(_role);
                    break;
                case RunnerType.AzureFunction:
                    role = RunnerFactory.CreateAzureFunctionRunner(_role);
                    break;
                case RunnerType.IisExpress:
                    role = RunnerFactory.CreateForWebSite(_role);
                    break;
                case RunnerType.Thread: //if rebuilt escalate to AppDomain
                    role = RunnerFactory.HasBeenReBuilt(Path.GetDirectoryName(_role.Config.Assembly)) ?
                        RunnerFactory.CreateAppDomainRunner(_role) :
                        RunnerFactory.CreateThreadRunner(_role);
                    break;
                case RunnerType.AppDomain:
                    role = RunnerFactory.CreateAppDomainRunner(_role);
                    break;
                case RunnerType.Custom:
                    role = RunnerFactory.CreateCustomRunner(_role);
                    break;
                //Our Config Validation should stop us ending here
                //but provide this backup anyway, not really a supported expectation
                case RunnerType.Unknown:
                default:
                    role = _role.IsIisExpress ?
                                RunnerFactory.CreateForWebSite(_role) :
                                RunnerFactory.HasBeenReBuilt(Path.GetDirectoryName(_role.Config.Assembly)) ?
                                    RunnerFactory.CreateAppDomainRunner(_role) :
                                    RunnerFactory.CreateThreadRunner(_role);
                    break;
            }

            _resources.Add(role);
            role.Start();
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
