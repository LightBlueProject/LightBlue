namespace LightBlue.MultiHost
{
    public interface IOpenFileDialogService
    {
        string OpenFileDialog(string filename, string title, string filter);
    }
}