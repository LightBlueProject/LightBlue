using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Serilog;

namespace LightBlue.Hosts
{
    public class WebHostProcess : IDisposable
    {
        private readonly ProcessStartInfo _start;
        private Process _process;
        private readonly string _name;

        public WebHostProcess(string exe,
            string arguments, 
            IEnumerable<KeyValuePair<string, string>> environmentVariables)
        {
            _start = new ProcessStartInfo(exe)
            {
                Arguments = arguments,
                LoadUserProfile = false,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            _name = Path.GetFileName(_start.FileName);

            Log.Information("{Process} start info with arguments {ProcessArguments}", _name, _start.Arguments);

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

            Task.Run(() =>
            {
                while (!_process.HasExited)
                {
                    var line = _process.StandardOutput.ReadLine();
                        
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        Log.Information("{Process} logged {Line}", _name, line);
                    }
                }
            });
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