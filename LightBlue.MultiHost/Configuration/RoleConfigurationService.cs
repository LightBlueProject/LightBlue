using LightBlue.MultiHost.ViewModel;

namespace LightBlue.MultiHost.Configuration
{
    public class RoleConfigurationService : IRoleConfiguationService
    {
        public bool Edit(string serviceTitle, string multiHostConfigurationFilePath)
        {
            var service = new MultiHostConfigurationService();
            var configuration = service.Load(multiHostConfigurationFilePath);
            var vm = new EditRole(serviceTitle, configuration, multiHostConfigurationFilePath);
            var view = new EditRoleView(vm);
            view.Show();
            return !view.Cancelled;
        }
    }
}