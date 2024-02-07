namespace LightBlue
{
    public interface IAzureCopyState
    {
        LightBlueBlobCopyStatus Status { get; }
        string StatusDescription { get; }
    }
}