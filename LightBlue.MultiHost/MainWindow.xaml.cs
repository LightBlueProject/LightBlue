using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using LightBlue.MultiHost.ViewModel;

namespace LightBlue.MultiHost
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private Role _selectedItem;
        private string _searchText;
        public CollectionViewSource CollectionViewSource { get; set; }
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

            CollectionViewSource = new CollectionViewSource();

            Services = new ObservableCollection<Role>();

            foreach (var h in App.Configuration.Roles)
            {
                var r = new Role(h);
                Services.Add(r);
                r.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == "Status") CollectionViewSource.View.Refresh();
                };
            }

            CollectionViewSource.Filter += CollectionViewSource_Filter;

            CollectionViewSource.Source = Services;

            DataContext = this;

            var autos = Services.Where(x => x.Status == RoleStatus.Sequenced).ToArray();
            BeginAutoStart(autos);
        }

        async void BeginAutoStart(Role[] roles)
        {
            await Task.Delay(TimeSpan.FromSeconds(0.2));
            foreach (var r in roles) r.StartAutomatically();
            //foreach (var r in roles.Take(10))
            //    r.StartAutomatically();
            //foreach (var r in roles.Skip(10))
            //{
            //    await TaskHelpers.AfterComputeOpportunities(1000000000, 500, CancellationToken.None);
            //    r.StartAutomatically();
            //}
        }

        void CollectionViewSource_Filter(object sender, FilterEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_searchText)) e.Accepted = true;
            else
            {
                var r = (Role)e.Item;
                e.Accepted = r.Title.ToLowerInvariant().Contains(_searchText.ToLowerInvariant())
                             || r.Status.ToString().ToLowerInvariant().Contains(_searchText.ToLowerInvariant())
                             || r.RoleName.ToLowerInvariant().Contains(_searchText.ToLowerInvariant());
            }
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

        private void FilterTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            _searchText = FilterTextBox.Text;
            CollectionViewSource.View.Refresh();
        }

        private void EditConfiguration_OnClick(object sender, RoutedEventArgs e)
        {
            if (VerifiySingleSelection())
            {
                var role = listView.SelectedItems.Cast<Role>().Single();
                var view = new ServiceConfigurationView(role);
                view.Show();
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
    }
}

