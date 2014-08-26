using System;

namespace LightBlue.Standalone
{
    internal static class UriExtensions
    {
        public static string RemoveToken(this Uri value)
        {
            var localPath = value.LocalPath;
            var index = localPath.IndexOf('?');

            return index < 0
                ? localPath
                : localPath.Substring(0, index);
        }
    }
}