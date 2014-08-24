using Microsoft.WindowsAzure.Storage.Blob;

namespace LightBlue.Standalone
{
    internal static class SharedAccessBlobPermissionsExtensions
    {
        public static string DeterminePermissionsString(this SharedAccessBlobPermissions permissions)
        {
            var permissionsString = "sp=";
            if ((permissions & SharedAccessBlobPermissions.Delete) == SharedAccessBlobPermissions.Delete)
            {
                permissionsString += "d";
            }
            if ((permissions & SharedAccessBlobPermissions.List) == SharedAccessBlobPermissions.List)
            {
                permissionsString += "l";
            }
            if ((permissions & SharedAccessBlobPermissions.Read) == SharedAccessBlobPermissions.Read)
            {
                permissionsString += "r";
            }
            if ((permissions & SharedAccessBlobPermissions.Write) == SharedAccessBlobPermissions.Write)
            {
                permissionsString += "w";
            }

            return permissionsString;
        }
    }
}