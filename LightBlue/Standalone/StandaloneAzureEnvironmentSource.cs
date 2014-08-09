namespace LightBlue.Standalone
{
    public class StandaloneAzureEnvironmentSource : IAzureEnvironmentSource
    {
        public AzureEnvironment CurrentEnvironment
        {
            get { return AzureEnvironment.LightBlue; }
        }
    }
}