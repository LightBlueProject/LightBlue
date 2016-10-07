using System.Windows;
using LightBlue.MultiHost.Core.ViewModel;

namespace LightBlue.MultiHost
{
    /// <summary>
    /// Interaction logic for RoleConfigurationView.xaml
    /// </summary>
    public partial class EditRoleView : Window
    {
        private readonly EditRole _vm;

        public bool Cancelled { get; set; }

        public EditRoleView()
        {
            InitializeComponent();
        }

        public EditRoleView(EditRole vm)
        {
            _vm = vm;
            DataContext = vm;
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
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
