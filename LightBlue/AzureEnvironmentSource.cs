namespace LightBlue
{
    public class AzureEnvironmentSource : IAzureEnvironmentSource
    {
        public AzureEnvironmentSource(AzureEnvironment currentEnvironment)
        {
            CurrentEnvironment = currentEnvironment;
        }

        public AzureEnvironment CurrentEnvironment { get; private set; }
    }
}