using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using LightBlue.MultiHost.Configuration;
using LightBlue.MultiHost.Controls;
using LightBlue.MultiHost.Runners;
using LightBlue.MultiHost.ViewModel.RoleStates;
using Brush = System.Windows.Media.Brush;

namespace LightBlue.MultiHost.ViewModel
{
    public class Role : INotifyPropertyChanged
    {
        private static readonly ConcurrentDictionary<RoleStatus, ImageSource> ImageCache = new ConcurrentDictionary<RoleStatus, ImageSource>();

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

        public ImageSource Image => ImageCache[Status];

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

            var iconSelector = GetIconSelector();
            foreach (var status in Enum.GetValues(typeof(RoleStatus)).Cast<RoleStatus>())
            {
                var brush = CustomBrushes.GetStatusColour(status, IsolationMode);
                ImageCache.AddOrUpdate(
                    status,
                    s => iconSelector(s, Config),
                    (s, source) => iconSelector(s, Config));
            }
        }

        private Func<RoleStatus, RoleConfiguration, ImageSource> GetIconSelector()
        {
            return (status, configuration) =>
            {
                string icon;

                switch (status)
                {
                    case RoleStatus.Starting:
                        icon = configuration?.IconLocations?.Starting;
                        break;
                    case RoleStatus.Running:
                        icon = configuration?.IconLocations?.Running;
                        break;
                    case RoleStatus.Stopped:
                        icon = configuration?.IconLocations?.Stopped;
                        break;
                    case RoleStatus.Stopping:
                        icon = configuration?.IconLocations?.Stopping;
                        break;
                    case RoleStatus.Recycling:
                        icon = configuration?.IconLocations?.Recycling;
                        break;
                    case RoleStatus.Sequenced:
                        icon = configuration?.IconLocations?.Sequenced;
                        break;
                    case RoleStatus.Crashing:
                        icon = configuration?.IconLocations?.Crashing;
                        break;
                    default:
                        icon = null;
                        break;
                }

                if (icon == null)
                {
                    switch (configuration?.RoleName)
                    {
                        case "ReadModelPopulator":
                            icon = @"Resources\readmodelpopulator.ico";
                            break;
                        case "CommandProcessor":
                            icon = @"Resources\domainservice.ico";
                            break;
                        case "ProcessManager":
                            icon = @"Resources\processmanager.ico";
                            break;
                        case "WebRole":
                            icon = configuration.Title.Contains("Hub")
                                ? @"Resources\messagehub.ico"
                                : @"Resources\website.ico";
                            break;
                        default:
                            icon = @"Resources\worker.ico";
                            break;
                    }

                    return ImageUtil.ColourImage(icon, CustomBrushes.GetStatusColour(status, IsolationMode));
                }

                return new BitmapImage(new Uri(icon));
            };
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