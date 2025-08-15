using System.ComponentModel;
using System.Linq;
using System.Windows.Media;
using LightBlue.MultiHost.Configuration;

namespace LightBlue.MultiHost.ViewModel
{
    public class EditRole : INotifyPropertyChanged
    {
        private readonly string _serviceTitle;
        private readonly MultiHostConfiguration _multiHostConfiguration;
        private readonly string _multiHostConfigurationFilePath;
        private ServiceConfiguration _roleConfiguration;

        public EditRole(string serviceTitle, MultiHostConfiguration multiHostConfiguration, string multiHostConfigurationFilePath)
        {
            _serviceTitle = serviceTitle;
            _multiHostConfiguration = multiHostConfiguration;
            _multiHostConfigurationFilePath = multiHostConfigurationFilePath;

            RoleConfiguration = _multiHostConfiguration.Services.Single(x => x.Title == _serviceTitle);
        }

        public ImageSource MainIcon { get { return ImageUtil.ColourImage(@"Resources\debug.ico", CustomBrushes.Main); } }

        public ServiceConfiguration RoleConfiguration
        {
            get { return _roleConfiguration; }
            set
            {
                _roleConfiguration = value;
                OnPropertyChanged("RoleConfiguration");
            }
        }

        public void BrowseToAssembly()
        {
            IOpenFileDialogService service = new OpenFileDialogService();
            RoleConfiguration.Assembly = service.OpenFileDialog(RoleConfiguration.Assembly, 
                "Choose service assembly",
                "Assembly Files (.dll)|*.dll");
            OnPropertyChanged("RoleConfiguration");
        }

        public void BrowseToServiceConfiguration()
        {
            IOpenFileDialogService service = new OpenFileDialogService();
            RoleConfiguration.ConfigurationPath = service.OpenFileDialog(RoleConfiguration.ConfigurationPath,
                "Choose configuration file",
                "Configuration Files (.config)|*.config");
        }

        public void SaveRoleConfiguration()
        {
            var service = new MultiHostConfigurationService();
            service.Save(_multiHostConfigurationFilePath, _multiHostConfiguration);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var d = PropertyChanged;
            if (d != null) d(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}