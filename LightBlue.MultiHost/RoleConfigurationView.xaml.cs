using System.Windows;

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

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Cancelled = true;
            Close();
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            _vm.UpdateRoleConfiguration();
            Close();
        }
    }
}
