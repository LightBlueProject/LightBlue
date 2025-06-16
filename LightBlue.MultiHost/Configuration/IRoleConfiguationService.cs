namespace LightBlue.MultiHost.Configuration
{
    public interface IRoleConfiguationService
    {
        ServiceConfiguration Edit(string serviceTitle, string multiHostConfigurationFilePath);
    }
}