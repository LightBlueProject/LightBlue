namespace LightBlue.Standalone
{
    public class StandaloneAzureBlobProperties : IAzureBlobProperties
    {
        public long Length { get; set; }
        public string ContentType { get; set; }
    }
}