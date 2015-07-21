using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using LightBlue.MultiHost.IISExpress;
using LightBlue.MultiHost.ViewModel;

namespace LightBlue.MultiHost.Runners
{
    class IisExpressRunner : IRunner
    {
        private readonly Role _role;
        private readonly TaskCompletionSource<object> _started = new TaskCompletionSource<object>();
        private readonly TaskCompletionSource<object> _completed = new TaskCompletionSource<object>();
        private readonly WebHostArgs _args;
        private Process _process;

        public Task Started { get { return _started.Task; } }
        public Task Completed { get { return _completed.Task; } }

        public IisExpressRunner(Role role)
        {
            _role = role;
            _args = WebConfigHelper.Create(role.Config);
            WebConfigHelper.PatchWebConfig(_args);
        }

        public void Start()
        {
            try
            {
                var iisExpressConfigurationFilePath = Path.Combine(Path.GetTempPath(),
                    Guid.NewGuid() + ".config");
                IisExpressHelper.GenerateIisExpressConfigurationFile(_args, iisExpressConfigurationFilePath);
                _process = new Process();
                _process.Exited += (s, e) => _completed.SetResult(new object());
                _process.EnableRaisingEvents = true;
                _process.StartInfo = IisExpressHelper.BuildProcessStartInfo(_args, iisExpressConfigurationFilePath);
                if (_process.Start())
                {
                    _role.TraceWriteLine("Website started in new IISExpress process: " + _process.Id);
                }
                else
                {
                    throw new NotSupportedException("James: I've not yet found a scenario where this can happen.");
                }
            }
            catch (Exception ex)
            {
                _role.TraceWriteLine("Could not start IIS process: " + ex.Message);
                _completed.SetResult(new object());
            }
            _started.SetResult(new object());
		}

        public bool Debug()
        {
            if (File.Exists(@"c:\\windows\\system32\\vsjitdebugger.exe"))
            {
                var psi = new ProcessStartInfo(@"c:\\windows\\system32\\vsjitdebugger.exe", "-p " + _process.Id);
                Process.Start(psi);
                return true;
            }
            return false;
        }

        public void Dispose()
        {
            if (_process != null)
            {
                try
                {
                    _process.Kill();
                    _role.TraceWriteLine("Terminated IIS Express process: " + _process.Id);
                }
                catch (InvalidOperationException ex)
                {
                    _role.TraceWriteLine("Couldn't terminate IIS Express process: " + ex.Message);
                }
            }
            else
            {
                _role.TraceWriteLine("Could not terminate IIS Express process");
            }
        }
    }
}