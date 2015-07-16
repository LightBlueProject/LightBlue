using System;
using System.IO;
using Microsoft.Win32;

namespace LightBlue.MultiHost
{
    public class OpenFileDialogService : IOpenFileDialogService
    {
        public string OpenFileDialog(string filename, string title, string filter)
        {
            var absoluteDirectoryPath = GetAbsoluteDirectoryPathToFile(filename);

            var dialog = new OpenFileDialog
            {
                Title = title,
                InitialDirectory = absoluteDirectoryPath,
                Filter = filter,
                CheckFileExists = true
            };

            var cancelled = !dialog.ShowDialog().GetValueOrDefault();
            if (cancelled)
            {
                return filename;
            }

            var relativePath = GetRelativePathToSelectedFile(dialog.FileName);
            return relativePath;
        }

        private static string GetAbsoluteDirectoryPathToFile(string filename)
        {
            var multiHostConfigurationPath = App.MultiHostConfigurationFilePath;
            var directoryName = Path.GetDirectoryName(multiHostConfigurationPath);
            var absoluteFilepath = Path.GetFullPath(Path.Combine(directoryName, filename));
            return Path.GetDirectoryName(absoluteFilepath);
        }

        private static string GetRelativePathToSelectedFile(string fileName)
        {
            var splitIndex = fileName.IndexOf("src", StringComparison.Ordinal);
            return "..\\" + fileName.Substring(splitIndex, fileName.Length - splitIndex);
        }
    }
}