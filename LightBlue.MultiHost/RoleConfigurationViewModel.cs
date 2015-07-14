using System.ComponentModel;
using System.Linq;
using LightBlue.MultiHost.Configuration;

namespace LightBlue.MultiHost
{
    public class RoleConfigurationViewModel : INotifyPropertyChanged
    {
        private readonly string _serviceTitle;
        private readonly MultiHostConfiguration _multiHostConfiguration;
        private readonly string _multiHostConfigurationFilePath;
        private RoleConfiguration _roleConfiguration;

        public RoleConfigurationViewModel(string serviceTitle, MultiHostConfiguration multiHostConfiguration, string multiHostConfigurationFilePath)
        {
            _serviceTitle = serviceTitle;
            _multiHostConfiguration = multiHostConfiguration;
            _multiHostConfigurationFilePath = multiHostConfigurationFilePath;

            RoleConfiguration = _multiHostConfiguration.Roles.Single(x => x.Title == _serviceTitle);
        }

        public RoleConfiguration RoleConfiguration
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
                "Choose service configuration",
                "Service Configuration Files (.cscfg)|*.cscfg");
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