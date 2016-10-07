namespace LightBlue.MultiHost.Core
{
    public interface IOpenFileDialogService
    {
        string OpenFileDialog(string filename, string title, string filter);
    }
}