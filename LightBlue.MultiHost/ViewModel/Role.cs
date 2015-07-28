using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Threading;

using LightBlue.MultiHost.Configuration;
using LightBlue.MultiHost.Controls;
using LightBlue.MultiHost.Runners;
using LightBlue.MultiHost.ViewModel.RoleStates;

namespace LightBlue.MultiHost.ViewModel
{
    public class Role : INotifyPropertyChanged
    {
        private static readonly ConcurrentDictionary<Tuple<string, SolidColorBrush>, ImageSource> ImageCache = new ConcurrentDictionary<Tuple<string, SolidColorBrush>, ImageSource>();

        public RoleStatus Status { get { return State.Status; } }

        public Brush StatusColor
        {
            get { return CustomBrushes.GetStatusColour(Status, IsolationMode); }
        }

        public string Title { get { return Config.Title; } }
        public string RoleName { get { return Config.RoleName; } }
        public bool IsIisExpress { get { return !string.IsNullOrWhiteSpace(Config.Hostname); } }

        private RoleIsolationMode? _isolationOverride;
        public RoleIsolationMode IsolationMode
        {
            get
            {
                if (_isolationOverride.HasValue)
                    return _isolationOverride.Value;

                return string.IsNullOrWhiteSpace(Config.RoleIsolationMode)
                    ? RoleIsolationMode.Thread
                    : (RoleIsolationMode)Enum.Parse(typeof(RoleIsolationMode), Config.RoleIsolationMode);
            }
            set
            {
                _isolationOverride = value;
                OnPropertyChanged("IsolationMode");
            }
        }

        public string ImageSource
        {
            get
            {
                // TODO: drive this from configuration
                switch (Config.RoleName)
                {
                    case "ReadModelPopulator":
                        return @"Resources\readmodelpopulator.ico";
                    case "CommandProcessor":
                        return @"Resources\domainservice.ico";
                    case "ProcessManager":
                        return @"Resources\processmanager.ico";
                    case "WebRole":
                        return Config.Title.Contains("Hub")
                            ? @"Resources\messagehub.ico"
                            : @"Resources\website.ico";
                    default:
                        return @"Resources\worker.ico";
                }
            }
        }

        public ImageSource Image
        {
            get
            {
                var brush = CustomBrushes.GetStatusColour(Status, IsolationMode);
                return ImageCache.GetOrAdd(Tuple.Create(ImageSource, brush), t => ImageUtil.ColourImage(t.Item1, t.Item2));
            }
        }

        public string DisplayText { get { return Config.Title; } }

        public ConcurrentDictionary<string, FifoLog> Logs
        {
            get { return _logs; }
            set
            {
                _logs = value;
                OnPropertyChanged("Logs");
            }
        }

        internal readonly RoleConfiguration Config;
        private readonly Dispatcher _dispatcher;
        private IState _state;
        private RoleRunner _current;
        private ConcurrentDictionary<string, FifoLog> _logs;

        internal IState State
        {
            get { return _state; }
            set
            {
                _state = value;
                OnPropertyChanged("Status");
                OnPropertyChanged("StatusColor");
                OnPropertyChanged("Image");
            }
        }

        public Role(RoleConfiguration role)
        {
            Config = role;
            State = Config.EnabledOnStartup ? (IState)new AutoStarting(this) : (IState)new Stopped(this);
            _dispatcher = Dispatcher.CurrentDispatcher;
            Logs = new ConcurrentDictionary<string, FifoLog>();
        }

        public void StartAutomatically()
        {
            State.StartAutomatically();
        }

        public void Start()
        {
            State.Start();
        }

        public void Started()
        {
            State.Started();
        }

        public void Crashed()
        {
            State.Crashed();
        }

        public void Stop()
        {
            State.Stop();
        }

        public void OnStopped()
        {
            State.OnStopped();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var d = PropertyChanged;
            if (d != null) d(this, new PropertyChangedEventArgs(propertyName));
        }

        internal async void StartInternal()
        {
            if (_current != null) throw new InvalidOperationException();
            TraceClear();
            _current = new RoleRunner(this);
            _current.Start();
            var original = _state;
            await _current.Started;
            if (original == _state) Started();
        }

        internal async void StartedInternal()
        {
            var original = _state;
            await _current.Completed;
            if (original == _state) Crashed();
        }

        internal async void StopInternal()
        {
            await Task.Run(() => _current.Dispose());
            _current = null;
            OnStopped();
        }

        internal async void RecycleInternal()
        {
            var original = _state;
            await Task.Delay(TimeSpan.FromSeconds(15));
            if (original == _state)
            {
                Start();
            }
        }

        public void TraceWrite(string runner, string s)
        {
            if (s.StartsWith("NewRelic")) return;

            RunnersLog(runner).Write(s);
        }

        private FifoLog RunnersLog(string runner)
        {
            FifoLog logger;
            if (Logs.TryGetValue(runner, out logger))
            {
                return logger;
            }
            return EnsureUiThread(() =>
            {
                var l = Logs.GetOrAdd(runner, s => new FifoLog(200));
                OnPropertyChanged("Logs");
                return l;
            });
        }

        public void TraceWriteLine(string runner, string s)
        {
            if (s.StartsWith("NewRelic")) return;

            RunnersLog(runner).WriteLine(s);
        }

        public void TraceClear()
        {
            foreach (var log in Logs.Values)
            {
                log.Clear();
            }
        }

        private T EnsureUiThread<T>(Func<T> a)
        {
            if (_dispatcher.CheckAccess())
            {
                return a();
            }
            return _dispatcher.Invoke(a);
        }

        public void DebugIisExpress()
        {
            if (_current != null)
            {
                _current.DebugIisExpress();
            }
        }
    }
}