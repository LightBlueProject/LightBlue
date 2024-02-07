namespace LightBlue
{
    public interface IAzureBlobProperties
    {
        long Length { get; }
        string ContentType { get; }
    }
}