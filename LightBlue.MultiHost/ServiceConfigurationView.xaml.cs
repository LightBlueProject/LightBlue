using System.Windows;
using LightBlue.MultiHost.ViewModel;

namespace LightBlue.MultiHost
{
    /// <summary>
    /// Interaction logic for ServiceConfiguration.xaml
    /// </summary>
    public partial class ServiceConfigurationView : Window
    {
        private readonly Role _vm;

        public ServiceConfigurationView()
        {
            InitializeComponent();
        }

        public ServiceConfigurationView(Role vm)
        {
            _vm = vm;
            DataContext = vm;
            InitializeComponent();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            _vm.UpdateServiceConfiguration();

            Close();
        }
    }
}
