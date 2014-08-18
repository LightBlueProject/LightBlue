using System;
using System.IO;

namespace LightBlue.Standalone
{
    public static class StandaloneEnvironment
    {
        public static string LightBlueDataDirectory
        {
            get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LightBlue"); }
        }
    }
}