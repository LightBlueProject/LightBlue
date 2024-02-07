using Azure.Storage.Blobs.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;


namespace LightBlue.Standalone
{
    internal static class StandaloneList
    {
        public static List<StandaloneAzureBlockBlob> ListBlobsSegmentedAsync(
            string containerDirectory,
            string prefix,
            BlobTraits blobTraits,
            int maxResults)
        {
            var resultSegment = FindFilesFlattened(containerDirectory, prefix, maxResults);

            if ((blobTraits & BlobTraits.Metadata) == BlobTraits.Metadata)
            {
                foreach (var blob in resultSegment.OfType<StandaloneAzureBlockBlob>())
                {
                    blob.FetchAttributes();
                }
            }

            return resultSegment;
        }

        private static List<StandaloneAzureBlockBlob> FindFilesFlattened(
            string containerDirectory,
            string prefix,
            int maxResults)
        {
            return new DirectoryInfo(containerDirectory).EnumerateFiles((prefix ?? "") + "*", SearchOption.AllDirectories)
                .Where(f => !(f.DirectoryName ?? "").EndsWith(".meta"))
                .Take(maxResults)
                .Select(f =>
                    new StandaloneAzureBlockBlob(
                        containerDirectory,
                        f.FullName.Substring(containerDirectory.Length + 1)))
                .ToList();
        }
    }
}