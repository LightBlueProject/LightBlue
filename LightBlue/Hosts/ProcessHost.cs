using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Serilog;

namespace LightBlue.Hosts
{
    public class ProcessHost : IDisposable
    {
        private readonly ProcessStartInfo _start;
        private Process _process;
        private readonly Action<string> _output;
        private readonly string _name;

        public ProcessHost(string exe,
            string arguments, 
            IEnumerable<KeyValuePair<string, string>> environmentVariables,
            Action<string> output)
        {
            _start = new ProcessStartInfo(exe)
            {
                Arguments = arguments,
                LoadUserProfile = false,
                UseShellExecute = false,
                RedirectStandardOutput = true
            };

            _name = Path.GetFileName(_start.FileName);

            Log.Information("{Process} start info with arguments {ProcessArguments}", _name, _start.Arguments);

            _output = output;

            foreach (var ev in environmentVariables)
            {
                if (_start.EnvironmentVariables.ContainsKey(ev.Key))
                    _start.EnvironmentVariables[ev.Key] = ev.Value;
                else
                    _start.EnvironmentVariables.Add(ev.Key, ev.Value);
            }
        }

        public void Start()
        {
            _process = Process.Start(_start);

            Log.Information("{Process}:{ProcessId} started with exit code {@ProcessExitCode}", _process.Id, _name, _process.ExitCode);

            if (_start.RedirectStandardOutput)
            {
                var reader = _process.StandardOutput;
                while (reader.Peek() > -1)
                {
                    var content = reader.ReadToEnd();
                    _output(content);
                    Log.Information("{Process}:{ProcessId} wrote output {@ProcessOutput}", _process.Id, _name, content);
                }
            }
        }

        public void Dispose()
        {
            if (_process == null || Process.GetProcesses().All(x => x.Id != _process.Id))
                return;

            _process.Kill();
            _process.WaitForExit();

            Log.Information("{Process}:{ProcessId} killed and disposed", _process.Id, _name);
        }
    }
}