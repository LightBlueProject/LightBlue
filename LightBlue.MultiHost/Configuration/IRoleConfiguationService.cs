namespace LightBlue.MultiHost.Configuration
{
    public interface IRoleConfiguationService
    {
        RoleConfiguration Edit(string serviceTitle, string multiHostConfigurationFilePath);
    }
}