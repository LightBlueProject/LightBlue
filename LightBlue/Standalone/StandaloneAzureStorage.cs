using System;
using System.IO;
using System.Linq;

namespace LightBlue.Standalone
{
    public class StandaloneAzureStorage : IAzureStorage
    {
        public const string DevelopmentAccountName = "dev";

        private readonly string _storageAccountDirectory;

        public StandaloneAzureStorage(string connectionString)
        {
            var storageAccountName = ExtractAccountName(connectionString);
            _storageAccountDirectory = Path.Combine(StandaloneEnvironment.LightBlueDataDirectory, storageAccountName);
            Directory.CreateDirectory(_storageAccountDirectory);
        }

        public IAzureBlobStorageClient CreateAzureBlobStorageClient()
        {
            return new StandaloneAzureBlobStorageClient(_storageAccountDirectory);
        }

        private static string ExtractAccountName(string connectionString)
        {
            var valuePairs = connectionString.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(component => component.Split(new[] {'='}, StringSplitOptions.RemoveEmptyEntries))
                .ToDictionary(c => c[0].ToLowerInvariant(), c => c[1]);

            if (valuePairs.ContainsKey("accountname"))
            {
                return valuePairs["accountname"];
            }
            if (valuePairs.ContainsKey("usedevelopmentstorage"))
            {
                return DevelopmentAccountName;
            }

            throw new FormatException("Could not parse the connection string.");
        }
    }
}