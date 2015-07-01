using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;

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

        public static string DeterminePermissionsString(this SharedAccessQueuePermissions permissions)
        {
            var permissionsString = "sp=";
            if ((permissions & SharedAccessQueuePermissions.Read) == SharedAccessQueuePermissions.Read)
            {
                permissionsString += "r";
            }
            if ((permissions & SharedAccessQueuePermissions.Add) == SharedAccessQueuePermissions.Add)
            {
                permissionsString += "a";
            }
            if ((permissions & SharedAccessQueuePermissions.Update) == SharedAccessQueuePermissions.Update)
            {
                permissionsString += "u";
            }
            if ((permissions & SharedAccessQueuePermissions.ProcessMessages) == SharedAccessQueuePermissions.ProcessMessages)
            {
                permissionsString += "p";
            }

            return permissionsString;
        }
    }
}