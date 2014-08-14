namespace LightBlue
{
    public interface IAzureLocalResourceSource
    {
        IAzureLocalResource this[string index] { get; }
    }
}