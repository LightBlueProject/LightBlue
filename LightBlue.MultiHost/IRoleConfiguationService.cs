namespace LightBlue.MultiHost
{
    public interface IRoleConfiguationService
    {
        bool Edit(string serviceTitle, string multiHostConfigurationFilePath);
    }
}