using System.Windows;
using LightBlue.MultiHost.Core.Configuration;
using LightBlue.MultiHost.Core.ViewModel;

namespace LightBlue.MultiHost
{
    public class RoleConfigurationService : IRoleConfiguationService
    {
        public RoleConfiguration Edit(string serviceTitle, string multiHostConfigurationFilePath)
        {
            var vm = new EditRole(serviceTitle, MultiHostRoot.Configuration, multiHostConfigurationFilePath);
            var view = new EditRoleView(vm)
            {
                Owner = Application.Current.MainWindow
            };
            view.ShowDialog();
            return view.Cancelled
                ? null
                : vm.RoleConfiguration;
        }
    }
}