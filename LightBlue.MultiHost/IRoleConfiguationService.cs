using LightBlue.MultiHost.Core.Configuration;

namespace LightBlue.MultiHost
{
    public interface IRoleConfiguationService
    {
        RoleConfiguration Edit(string serviceTitle, string multiHostConfigurationFilePath);
    }
}