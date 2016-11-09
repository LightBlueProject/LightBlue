using System;
using System.IO;

namespace LightBlue.Infrastructure
{
    public class LightBlueFileSystem
    {
        public static DirectoryInfo LocalAppData
        {
            get
            {
                var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                return new DirectoryInfo(Path.Combine(localAppData, "LightBlue", "temp"));
            }
        }
    }
}