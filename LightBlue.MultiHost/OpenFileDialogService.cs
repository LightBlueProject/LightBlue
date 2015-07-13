using Microsoft.Win32;

namespace LightBlue.MultiHost
{
    public class OpenFileDialogService : IOpenFileDialogService
    {
        public string OpenFileDialog(string filename, string title, string filter)
        {
            var dialog = new OpenFileDialog
            {
                Title = title,
                Filter = filter,
                CheckFileExists = true
            };

            var filepath = dialog.ShowDialog().GetValueOrDefault()
                ? dialog.FileName
                : filename;

            return filepath;
        }
    }
}