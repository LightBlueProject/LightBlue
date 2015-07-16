using System.Windows;
using LightBlue.MultiHost.ViewModel;

namespace LightBlue.MultiHost
{
    /// <summary>
    /// Interaction logic for RoleConfigurationView.xaml
    /// </summary>
    public partial class RoleConfigurationView : Window
    {
        private readonly RoleConfigurationViewModel _vm;

        public bool Cancelled { get; set; }

        public RoleConfigurationView()
        {
            InitializeComponent();
        }

        public RoleConfigurationView(RoleConfigurationViewModel vm)
        {
            _vm = vm;
            DataContext = vm;
            InitializeComponent();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Cancelled = true;
            Close();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            _vm.SaveRoleConfiguration();
            Close();
        }

        private void BrowseToAssemblyButton_Click(object sender, RoutedEventArgs e)
        {
            _vm.BrowseToAssembly();
        }

        private void BrowseToServerConfigurationButton_Click(object sender, RoutedEventArgs e)
        {
            _vm.BrowseToServiceConfiguration();
        }
    }
}
