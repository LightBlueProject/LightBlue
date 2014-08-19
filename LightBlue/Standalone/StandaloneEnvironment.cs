using System;
using System.IO;

namespace LightBlue.Standalone
{
    public static class StandaloneEnvironment
    {
        private static readonly string _lightBlueDataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LightBlue");

        public static string LightBlueDataDirectory
        {
            get { return _lightBlueDataDirectory; }
        }
    }
}