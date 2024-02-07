using Azure.Storage.Sas;

namespace LightBlue.Standalone
{
    internal static class SharedAccessBlobPermissionsExtensions
    {
        public static string DeterminePermissionsString(this BlobContainerSasPermissions permissions)
        {
            var permissionsString = "sp=";
            if ((permissions & BlobContainerSasPermissions.Delete) == BlobContainerSasPermissions.Delete)
            {
                permissionsString += "d";
            }
            if ((permissions & BlobContainerSasPermissions.List) == BlobContainerSasPermissions.List)
            {
                permissionsString += "l";
            }
            if ((permissions & BlobContainerSasPermissions.Read) == BlobContainerSasPermissions.Read)
            {
                permissionsString += "r";
            }
            if ((permissions & BlobContainerSasPermissions.Write) == BlobContainerSasPermissions.Write)
            {
                permissionsString += "w";
            }

            return permissionsString;
        }

        public static string DeterminePermissionsString(this BlobSasPermissions permissions)
        {
            var permissionsString = "sp=";
            if ((permissions & BlobSasPermissions.Delete) == BlobSasPermissions.Delete)
            {
                permissionsString += "d";
            }
            if ((permissions & BlobSasPermissions.List) == BlobSasPermissions.List)
            {
                permissionsString += "l";
            }
            if ((permissions & BlobSasPermissions.Read) == BlobSasPermissions.Read)
            {
                permissionsString += "r";
            }
            if ((permissions & BlobSasPermissions.Write) == BlobSasPermissions.Write)
            {
                permissionsString += "w";
            }

            return permissionsString;
        }

        public static string DeterminePermissionsString(this QueueSasPermissions permissions)
        {
            var permissionsString = "sp=";
            if ((permissions & QueueSasPermissions.Read) == QueueSasPermissions.Read)
            {
                permissionsString += "r";
            }
            if ((permissions & QueueSasPermissions.Add) == QueueSasPermissions.Add)
            {
                permissionsString += "a";
            }
            if ((permissions & QueueSasPermissions.Update) == QueueSasPermissions.Update)
            {
                permissionsString += "u";
            }
            if ((permissions & QueueSasPermissions.Process) == QueueSasPermissions.Process)
            {
                permissionsString += "p";
            }

            return permissionsString;
        }
    }
}