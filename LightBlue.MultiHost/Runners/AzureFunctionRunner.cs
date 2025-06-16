using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using LightBlue.MultiHost.ViewModel;

namespace LightBlue.MultiHost.Runners
{
    public class AzureFunctionRunner : IRunner
    {
        private readonly Role _role;
        private Process _parent;

        public string Identifier { get; }

        public Task Started => _started.Task;
        public Task Completed => _completed.Task;

        private readonly TaskCompletionSource<object> _started = new TaskCompletionSource<object>();
        private readonly TaskCompletionSource<object> _completed = new TaskCompletionSource<object>();

        public AzureFunctionRunner(Role role)
        {
            Identifier = "AzureFunction: " + role.RoleName;

            _role = role;
        }

        public void Start()
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Microsoft", "Azure Functions Core Tools", "func.exe");
            if (!File.Exists(path))
                throw new InvalidOperationException($"Azure Functions Core Tools must be installed at {path}");

            _parent = _role.NewDefaultProcess(path, $"start --port {_role.Config.Port}");

            _parent.OutputDataReceived += (_, args) =>
            {
                if (!string.IsNullOrWhiteSpace(args.Data))
                    _role.TraceWriteLine(Identifier, args.Data);
            };
            _parent.ErrorDataReceived += (_, args) =>
            {
                if (!string.IsNullOrWhiteSpace(args.Data))
                    _role.TraceWriteLine(Identifier, args.Data);
            };
            _parent.Exited += (s, e) =>
            {
                _role.TraceWriteLine(Identifier, $"Parent process {_role.RoleName} exited");
                _completed.SetResult(new object());
            };

            _parent.Start();
            _parent.BeginOutputReadLine();
            _parent.BeginErrorReadLine();
            _parent.SetPriority(_role);
            _parent.AllocateToMultiHostProcess();

            _started.SetResult(new object());

            _role.TraceWriteLine(Identifier, $"Process {_parent.Id} {_parent.ProcessName} {_parent.StartInfo.Arguments} {_parent.StartInfo.WorkingDirectory} started");
        }

        public void Dispose()
        {
            foreach (var c in _parent.GetChildren())
            {
                if (!c.HasExited)
                {
                    _role.TraceWriteLine(Identifier, $"Kill child process {c.Id} {c.ProcessName}");
                    c.Kill();
                }
            }

            if (!_parent.HasExited)
            {
                _role.TraceWriteLine(Identifier, $"Kill parent process {_parent.Id} {_parent.ProcessName} {_parent.StartInfo.Arguments} {_parent.StartInfo.WorkingDirectory}");
                _parent.Kill();
            }
        }
    }
}