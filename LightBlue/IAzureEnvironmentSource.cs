namespace LightBlue
{
    public interface IAzureEnvironmentSource
    {
        AzureEnvironment CurrentEnvironment { get; }
    }
}
