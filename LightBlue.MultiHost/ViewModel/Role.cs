using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LightBlue.MultiHost.Configuration;
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

        public string Trace
        {
            get { return _trace; }
            set
            {
                _trace = value;
                OnPropertyChanged("Trace");
            }
        }

        public ObservableCollection<string> TraceElements { get; set; }

        public TextBox TraceBox { get; private set; }

        public RoleConfiguration Config { get; private set; }

        private IState _state;

        private RoleRunner _current;
        private string _trace;

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
            TraceBox = new TextBox
            {
                TextWrapping = TextWrapping.Wrap,
                Background = Brushes.Black,
                Foreground = Brushes.LightGray,
                VerticalScrollBarVisibility = ScrollBarVisibility.Visible,
                IsReadOnly = true,
                FontFamily = new FontFamily("Consolas"),
                FontSize = 14,
                AcceptsReturn = true,
            };
            TraceElements = new ObservableCollection<string>();
            TraceWriteLine(role.Title + " configuration loaded...\r\n");

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

        public void TraceWrite(string s)
        {
            if (s.StartsWith("NewRelic")) return;
            EnsureUIThread(() =>
            {
                Trace += s;

                if (TraceElements.Count > 0)
                {
                    var index = TraceElements.Count - 1;
                    var msg = TraceElements[index];
                    TraceElements.RemoveAt(index);
                    TraceElements.Add(msg + s);
                }
                else
                {
                    TraceElements.Add(s);
                }

                TraceBox.AppendText(s);
                TraceBox.ScrollToEnd();
            });
        }

        public void TraceWriteLine(string s)
        {
            if (s.StartsWith("NewRelic")) return;
            EnsureUIThread(() =>
            {
                Trace += s + "\r\n";
                TraceElements.Add(s);
                TraceBox.AppendText(s + "\r\n");
                TraceBox.ScrollToEnd();
            });
        }

        public void TraceClear()
        {
            EnsureUIThread(() =>
            {
                Trace = "";
                TraceElements.Clear();
                TraceBox.Clear();
            });
        }

        private void EnsureUIThread(Action a)
        {
            if (TraceBox.Dispatcher.CheckAccess())
            {
                a();
            }
            else
            {
                TraceBox.Dispatcher.Invoke(a);
            }
        }

        public void UpdateServiceConfiguration()
        {
            var service = new ServiceConfiguationService();
            service.Edit(null, null);
        }
    }
}