namespace LightBlue
{
    public interface IAzureLocalResource
    {
        int MaximumSizeInMegabytes { get; }
        string Name { get; }
        string RootPath { get; }
    }
}