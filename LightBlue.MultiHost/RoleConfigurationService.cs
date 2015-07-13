using System.ComponentModel;
using System.IO;
using System.Linq;
using LightBlue.MultiHost.Configuration;
using Newtonsoft.Json;

namespace LightBlue.MultiHost
{
    public class RoleConfigurationService : IRoleConfiguationService
    {
        public bool Edit(string serviceTitle, string multiHostConfigurationFilePath)
        {
            var jsonText = File.ReadAllText(multiHostConfigurationFilePath);

            var multiHostConfiguration = JsonConvert.DeserializeObject<MultiHostConfiguration>(jsonText);

            var roleConfiguration = multiHostConfiguration.Roles.Single(x => x.Title == serviceTitle);

            var view = new RoleConfigurationView(new RoleConfigurationViewModel(serviceTitle, multiHostConfiguration, multiHostConfigurationFilePath));

            view.Show();

            if (view.Cancelled)
            {
                return false;
            }



            return false;
        }
    }

    public class RoleConfigurationViewModel : INotifyPropertyChanged
    {
        private readonly string _serviceTitle;
        private readonly MultiHostConfiguration _multiHostConfiguration;
        private readonly string _multiHostConfigurationFilePath;

        public RoleConfiguration Configuration
        {
            get { return _multiHostConfiguration.Roles.Single(x => x.Title == _serviceTitle); }
        }

        public RoleConfigurationViewModel(string serviceTitle, MultiHostConfiguration multiHostConfiguration, string multiHostConfigurationFilePath)
        {
            _serviceTitle = serviceTitle;
            _multiHostConfiguration = multiHostConfiguration;
            _multiHostConfigurationFilePath = multiHostConfigurationFilePath;
        }

        public void UpdateRoleConfiguration()
        {
            using (var fs = new FileStream(_multiHostConfigurationFilePath, FileMode.Truncate, FileAccess.Write))
            using (var sw = new StreamWriter(fs))
            {
                sw.WriteLine(JsonConvert.SerializeObject(_multiHostConfiguration));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var d = PropertyChanged;
            if (d != null) d(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}