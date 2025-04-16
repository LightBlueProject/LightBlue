using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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

        public event EventHandler<EventArgs> RolePanic;

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

        public ProcessPriority ProcessPriority
        {
            get
            {
                return string.IsNullOrWhiteSpace(Config.ProcessPriority)
                    ? ProcessPriority.BelowNormal
                    : (ProcessPriority)Enum.Parse(typeof(ProcessPriority), Config.ProcessPriority);
            }
        }

        public string ImageSource
        {
            get
            {
                var iconOption = App.Configuration.CustomRunners.FirstOrDefault(x => x.Name == Config.RoleName) is CustomRunnerConfiguration customRunner
                    ? customRunner.Icon
                    : IconHelper.RoleToIconOption(Config);

                return IconHelper.IconPaths[iconOption];
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

        public ObservableCollection<FifoLog> Logs
        {
            get { return _logs; }
        }

        internal readonly RoleConfiguration Config;
        private readonly Dispatcher _dispatcher;
        private IState _state;
        private RoleRunner _current;
        private readonly ObservableCollection<FifoLog> _logs;

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
            State = Config.EnabledOnStartup ? new AutoStarting(this) : (IState)new Stopped(this);
            _dispatcher = Dispatcher.CurrentDispatcher;
            _logs = new ObservableCollection<FifoLog>();
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
            NotificationHub.Notify(Config.Title, "This role has crashed and will be recycled", OnPanic);
        }

        private void OnPanic()
        {
            if (RolePanic != null)
            {
                RolePanic(this, EventArgs.Empty);
            }
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
            Func<FifoLog> addNewLoggerOnUiThread = () => EnsureUiThread(() =>
            {
                var logger = new FifoLog(runner, 200);
                Logs.Add(logger);
                return logger;
            });

            lock (Logs)
            {
                var fifoLog = GetLogger(runner);
                return fifoLog ?? addNewLoggerOnUiThread();
            }
        }

        private FifoLog GetLogger(string runner)
        {
            var logger = Logs.SingleOrDefault(x => x.LogName == runner);
            if (logger != null)
            {
                return logger;
            }
            return null;
        }

        public void TraceWriteLine(string runner, string s)
        {
            if (s.StartsWith("NewRelic")) return;

            RunnersLog(runner).WriteLine(s);
        }

        public void TraceClear()
        {
            lock (Logs)
            {
                foreach (var log in Logs)
                {
                    log.Clear();
                }
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