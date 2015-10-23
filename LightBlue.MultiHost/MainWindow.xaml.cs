using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using LightBlue.MultiHost.Configuration;
using LightBlue.MultiHost.Controls;
using LightBlue.MultiHost.ViewModel;

namespace LightBlue.MultiHost
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow :  INotifyPropertyChanged
    {
        private const UInt32 FLASHW_ALL = 3; //Flash both the window caption and taskbar button.        
        private const UInt32 FLASHW_TIMER = 4; //Flash continuously, until the FLASHW_STOP flag is set.        

        [StructLayout(LayoutKind.Sequential)]
        private struct FLASHWINFO
        {
            public UInt32 cbSize; //The size of the structure in bytes.            
            public IntPtr hwnd; //A Handle to the Window to be Flashed. The window can be either opened or minimized.
            public UInt32 dwFlags; //The Flash Status.            
            public UInt32 uCount; // number of times to flash the window            
            public UInt32 dwTimeout; //The rate at which the Window is to be flashed, in milliseconds. If Zero, the function uses the default cursor blink rate.        
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool FlashWindowEx(ref FLASHWINFO pwfi);

        private Role _selectedItem;
        private string _searchText;
        public ListCollectionViewEx CollectionViewSource { get; set; }
        public ObservableCollection<Role> Services { get; set; }
        public ImageSource MainIcon { get { return ImageUtil.ColourImage(@"Resources\debug.ico", CustomBrushes.Main); } }

        public Role SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                OnPropertyChanged("SelectedItem");
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                string msg = null;
                try
                {
                    msg = e.ExceptionObject.ToString();
                }
                catch (Exception ex)
                {
                    msg = "Couldn't get exception details: " + ex.Message;
                }

                try
                {
                    MessageBox.Show(msg);
                }
                catch
                {
                    Debugger.Launch();
                    Debugger.Break();
                }
            };
            App.Current.DispatcherUnhandledException += (s, e) =>
            {
                string msg = null;
                try
                {
                    msg = e.Exception.ToString();
                }
                catch (Exception ex)
                {
                    msg = "Couldn't get exception details: " + ex.Message;
                }

                try
                {
                    MessageBox.Show(msg);
                }
                catch
                {
                    Debugger.Launch();
                    Debugger.Break();
                }
            };

            Services = new ObservableCollection<Role>();
            CollectionViewSource = new ListCollectionViewEx(Services);

            foreach (var h in App.Configuration.Roles)
            {
                var r = new Role(h);
                Services.Add(r);
                r.RoleCrashed += OnRoleCrashed;
                r.RolePanic += OnRolePanicking;
                r.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == "Status") CollectionViewSource.Refresh();
                };
            }

            CollectionViewSource.Filter += CollectionViewSource_Filter;

            DataContext = this;

            var autos = Services.Where(x => x.Status == RoleStatus.Sequenced).ToArray();
            BeginAutoStart(autos);

            Loaded += (s, a) =>
            {
                NotificationHub.Initialise(this);
            };
        }

        private void OnRoleCrashed(object sender, EventArgs eventArgs)
        {
            FlashWindow(5);
        }

        private void OnRolePanicking(object sender, EventArgs e)
        {
            var roleSender = sender as Role;
            if (roleSender == null)
            {
                return;
            }

            if (WindowState == WindowState.Minimized)
            {
                WindowState = WindowState.Normal;
                Activate();
            }
            SelectedItem = roleSender;
            listView.ScrollIntoView(roleSender);
        }

        public string Version
        {
            get
            {
                return Assembly.GetExecutingAssembly()
                    .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                    .InformationalVersion;
            }
        }

        async void BeginAutoStart(Role[] roles)
        {
            await Task.Delay(TimeSpan.FromSeconds(0.2));
            foreach (var r in roles) r.StartAutomatically();
        }

        bool CollectionViewSource_Filter(object item)
        {
            if (string.IsNullOrWhiteSpace(_searchText)) return true;
            
            var r = (Role) item;
            var filter = r.Title.ToLowerInvariant().Contains(_searchText.ToLowerInvariant())
                         || r.Status.ToString().ToLowerInvariant().Contains(_searchText.ToLowerInvariant())
                         || r.RoleName.ToLowerInvariant().Contains(_searchText.ToLowerInvariant());
            return filter;
        }

        private void Start_OnClick(object sender, RoutedEventArgs e)
        {
            if (VerifiySelection())
            {
                var copy = listView.SelectedItems.Cast<Role>().ToArray();
                foreach (var r in copy)
                {
                    r.Start();
                }
            }
        }

        private bool VerifiySelection()
        {
            if (listView.SelectedItems.Count == 0)
            {
                MessageBox.Show("Can't perform action: you don't have any services selected.");
                return false;
            }
            return true;
        }

        private void Stop_OnClick(object sender, RoutedEventArgs e)
        {
            if (VerifiySelection())
            {
                var copy = listView.SelectedItems.Cast<Role>().ToArray();
                foreach (var r in copy)
                {
                    r.Stop();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var d = PropertyChanged;
            if (d != null) d(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Debug_OnClick(object sender, RoutedEventArgs e)
        {
            Debugger.Launch();
        }

        private void DebugIis_OnClick(object sender, RoutedEventArgs e)
        {
            if (SelectedItem != null && SelectedItem.IsIisExpress)
            {
                SelectedItem.DebugIisExpress();
            }
        }

        private void FilterTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            _searchText = FilterTextBox.Text;
            CollectionViewSource.Refresh();
        }

        private void EditConfiguration_OnClick(object sender, RoutedEventArgs e)
        {
            if (VerifiySingleSelection())
            {
                var role = (Role)listView.SelectedItem;
                if (role.Status == RoleStatus.Stopped)
                {
                    var service = new RoleConfigurationService();
                    var changed = service.Edit(role.Title, App.MultiHostConfigurationFilePath);
                    if (changed != null)
                    {
                        changed.EnabledOnStartup = false;

                        var configDir = Path.GetDirectoryName(App.MultiHostConfigurationFilePath);
                        changed.ConfigurationPath = Path.GetFullPath(Path.Combine(configDir, changed.ConfigurationPath));
                        changed.Assembly = Path.GetFullPath(Path.Combine(configDir, changed.Assembly));

                        var newRole = new Role(changed);
                        
                        var index = listView.SelectedIndex;
                        Services.RemoveAt(index);
                        Services.Insert(index, newRole);

                        newRole.RolePanic += OnRolePanicking;

                        listView.SelectedIndex = index;
                    }
                }
                else
                {
                    MessageBox.Show(role.Title + " must be stopped if you want to edit its configuration.");
                }
            }
        }

        private bool VerifiySingleSelection()
        {
            if (listView.SelectedItems.Count == 0)
            {
                MessageBox.Show("Can't perform action: you don't have any services selected.");
                return false;
            }
            if (listView.SelectedItems.Count > 1)
            {
                MessageBox.Show("Can't perform action: you have more than one service selected.");
                return false;
            }
            return true;
        }

        public void FlashWindow(UInt32 count = UInt32.MaxValue)
        {
            var h = new WindowInteropHelper(this);
            var info = new FLASHWINFO
            {
                hwnd = h.Handle,
                dwFlags = FLASHW_ALL | FLASHW_TIMER,
                uCount = count,
                dwTimeout = 0
            };

            info.cbSize = Convert.ToUInt32(Marshal.SizeOf(info));
            FlashWindowEx(ref info);
        }
    }
}

