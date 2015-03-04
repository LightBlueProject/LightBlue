using System;
using System.IO;

namespace LightBlue.Standalone
{
    public static class StandaloneEnvironment
    {
        private static string _lightBlueDataDirectory;

        static StandaloneEnvironment()
        {
            SetStandardLightBlueDataDirectory();
        }

        public static string LightBlueDataDirectory
        {
            get { return _lightBlueDataDirectory; }
            set
            {
                _lightBlueDataDirectory = value.EndsWith(Path.DirectorySeparatorChar.ToString())
                    ? value
                    : value + Path.DirectorySeparatorChar;
            }
        }

        public static void SetStandardLightBlueDataDirectory()
        {
            _lightBlueDataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LightBlue" + Path.DirectorySeparatorChar);;
        }

        public static BlobLocationParts SeparateBlobUri(Uri blobUri)
        {
            if (blobUri == null)
            {
                throw new ArgumentNullException("blobUri");
            }
            if (blobUri.Scheme != "file")
            {
                throw new ArgumentException("Only file Uris can be separated", "blobUri");
            }

            var localPath = blobUri.GetLocalPathWithoutToken();

            if (!localPath.StartsWith(_lightBlueDataDirectory))
            {
                throw new ArgumentException("Blob must be located in the LightBlue data directory", "blobUri");
            } 
            var subsection = localPath.Substring(_lightBlueDataDirectory.Length);

            var index = IndexOfNth(subsection, Path.DirectorySeparatorChar, 3);

            return new BlobLocationParts(
                Path.Combine(_lightBlueDataDirectory, subsection.Substring(0, index)),
                subsection.Substring(index + 1));
        }

        private static int IndexOfNth(string value, char character, int numberToFind)
        {
            var index = -1;

            for (var i = 0; i < numberToFind; i++)
            {
                index = value.IndexOf(character, index + 1);

                if (index == -1)
                {
                    break;
                }
            }

            return index;
        }
    }
}