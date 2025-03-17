using System.Diagnostics;
using System.Threading.Tasks;
using LightBlue.MultiHost.ViewModel;

namespace LightBlue.MultiHost.Runners
{
    public class DotNetCoreRunner : IRunner
    {
        private readonly Role _role;
        private Process _parent;

        public string Identifier { get; }

        public Task Started => _started.Task;
        public Task Completed => _completed.Task;

        private readonly TaskCompletionSource<object> _started = new TaskCompletionSource<object>();
        private readonly TaskCompletionSource<object> _completed = new TaskCompletionSource<object>();

        public DotNetCoreRunner(Role role)
        {
            Identifier = role.RoleName;

            _role = role;
        }

        public void Start()
        {
            _parent = CreateProcessFrom(_role);

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

        private static Process CreateProcessFrom(Role role)
        {
            if (role.Config.Assembly.EndsWith(".exe"))
            {
                // Run the output .exe directly
                return role.NewDefaultProcess(role.Config.Assembly, "");
            }
            else if (role.Config.Assembly.EndsWith(".dll"))
            {
                // Run the output .dll directly
                return role.NewDefaultProcess("dotnet", role.Config.Assembly);
            }
            else
            {
                // Run the project with dotnet run
                return role.NewDefaultProcess("dotnet", "run --no-build --no-restore");
            }
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