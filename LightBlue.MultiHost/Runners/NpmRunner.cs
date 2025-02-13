using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LightBlue.MultiHost.ViewModel;

namespace LightBlue.MultiHost.Runners
{
    public class NpmRunner : IRunner
    {
        private readonly Role _role;

        public string Identifier { get; }

        public Task Started => _started.Task;
        public Task Completed => _completed.Task;

        private readonly TaskCompletionSource<object> _started = new TaskCompletionSource<object>();
        private readonly TaskCompletionSource<object> _completed = new TaskCompletionSource<object>();
        private Process _parent;

        public NpmRunner(Role role)
        {
            Identifier = role.RoleName;

            _role = role;
        }

        public void Start()
        {
            var path = GetNpmPath();

            _parent = new Process
            {
                EnableRaisingEvents = true,
                StartInfo = new ProcessStartInfo(path, "start")
                {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = Path.GetDirectoryName(_role.Config.ConfigurationPath)
                }
            };
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

        private static string GetNpmPath()
        {
            // Depending on the installation, npm can be in different locations
            var paths = new[]
            {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "npm", "npm.cmd"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "nodejs", "npm.cmd")
            };

            var npmPath = paths.FirstOrDefault(File.Exists);

            if (npmPath == default)
                throw new InvalidOperationException($"NodeJS must be installed. Checked paths:{Environment.NewLine}{string.Join(Environment.NewLine, paths)}");

            return npmPath;
        }
    }
}