namespace LightBlue.MultiHost.Configuration
{
    public interface IRoleConfiguationService
    {
        bool Edit(string serviceTitle, string multiHostConfigurationFilePath);
    }
}