namespace LightBlue.Standalone
{
    public class StandaloneAzureLocalResource : IAzureLocalResource
    {
        public int MaximumSizeInMegabytes { get; set; }
        public string Name { get; set; }
        public string RootPath { get; set; }
    }
}